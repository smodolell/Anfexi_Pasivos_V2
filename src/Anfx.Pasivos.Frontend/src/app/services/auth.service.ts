import { Injectable, Inject, isDevMode } from '@angular/core';
import { BehaviorSubject, ReplaySubject, firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { API_BASE_URL } from '../api.config';

export interface User {
  id: number;
  nombreCompleto: string;
  email: string;
  usuarioNombre: string;
  role: 'Webmaster' | 'Admin' | 'User';
}

export interface LoginCredentials {
  email: string;
  contrasenia: string;
  recuerdame?: boolean;
}

export type LoginResult = { success: boolean; message: string; user?: User; errors?: string[] };
export type ApiLoginResponse = { token: string; user: User; message?: string; errors?: string[] };

/** Segundos antes de expirar en los que se lanza el refresh proactivo */
const REFRESH_BEFORE_EXPIRY_S = 60;

@Injectable({ providedIn: 'root' })
export class AuthService {
  // ── Estado ────────────────────────────────────────────────
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  readonly currentUser$ = this.currentUserSubject.asObservable();
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  // ── Token en memoria (también persiste en storage para restaurar sesión al recargar)
  private _token: string | null = null;

  // ── Refresh proactivo ─────────────────────────────────────
  private refreshTimer: ReturnType<typeof setTimeout> | null = null;
  private refreshInProgress = false;
  private readonly refreshQueue = new ReplaySubject<boolean>(1);

  // ── Cross-tab logout ──────────────────────────────────────
  private storageListenerRegistered = false;
  /** Clave usada SOLO para señalizar logout entre pestañas (no guarda el token) */
  private static readonly SESSION_KEY = 'pf_session';

  constructor(
    private readonly router: Router,
    private readonly http: HttpClient,
    @Inject(API_BASE_URL) private readonly apiBaseUrl: string,
  ) {
    this.checkStoredAuth();
    this.setupStorageListener();
  }

  // ── Login ─────────────────────────────────────────────────

  async login(credentials: LoginCredentials): Promise<LoginResult> {
    try {
      const res = await firstValueFrom(
        this.http.post<ApiLoginResponse>(`${this.apiBaseUrl}/auth/login`, credentials),
      );

      this._token = res.token;
      this.currentUserSubject.next(res.user);
      this.isAuthenticatedSubject.next(true);

      // Persiste SOLO el usuario (no el token) para restaurar UI entre recargas
      const storage = credentials.recuerdame ? localStorage : sessionStorage;
      storage.setItem('pf_token', res.token);
      storage.setItem('currentUser', JSON.stringify(res.user));
      // Señal de sesión activa para cross-tab sync
      localStorage.setItem(AuthService.SESSION_KEY, '1');

      this.scheduleProactiveRefresh();

      return { success: true, message: res.message ?? 'Login exitoso', user: res.user };
    } catch (err) {
      const error = err as HttpErrorResponse;
      const message =
        error.status === 0
          ? 'No se pudo conectar con el servidor. Verifique su conexión a internet.'
          : (error.error?.message ?? 'Error en el servidor');
      return { success: false, message, errors: error.error?.errors ?? [] };
    }
  }

  // ── Logout ────────────────────────────────────────────────

  logout(): void {
    this.clearAuth();
    this.router.navigate(['/auth/login']);
  }

  // ── Estado ────────────────────────────────────────────────

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
  isAdmin(): boolean {
    const role = this.currentUserSubject.value?.role;
    return role === 'Admin' || role === 'Webmaster';
  }
  hasRole(role: string): boolean {
    return this.currentUserSubject.value?.role === role;
  }

  async requestPasswordRecovery(email: string): Promise<void> {
    await firstValueFrom(this.http.post(`${this.apiBaseUrl}/auth/recovery`, { email }));
  }

  // ── Token ─────────────────────────────────────────────────

  /** Retorna el token en memoria — nunca desde storage */
  getAuthToken(): string | null {
    return this._token;
  }

  isTokenExpired(token?: string): boolean {
    const t = token ?? this._token;
    if (!t) return true;
    try {
      const parts = t.split('.');
      if (parts.length !== 3) return true;
      const payload = JSON.parse(atob(parts[1])) as { exp?: number };
      return payload.exp != null && payload.exp < Math.floor(Date.now() / 1000);
    } catch {
      return true;
    }
  }

  // ── Guard helper ──────────────────────────────────────────

  async checkAuthentication(): Promise<boolean> {
    if (this._token && !this.isTokenExpired()) return this.isAuthenticated();
    // Token expirado o ausente en memoria → limpiar
    this.clearAuth();
    return false;
  }

  // ── Refresh proactivo ─────────────────────────────────────

  private scheduleProactiveRefresh(): void {
    this.cancelRefreshTimer();
    if (!this._token) return;

    try {
      const parts = this._token.split('.');
      const payload = JSON.parse(atob(parts[1])) as { exp?: number };
      if (!payload.exp) return;

      const msUntilRefresh =
        (payload.exp - Math.floor(Date.now() / 1000) - REFRESH_BEFORE_EXPIRY_S) * 1000;
      if (msUntilRefresh <= 0) {
        this.performRefresh();
        return;
      }

      this.refreshTimer = setTimeout(() => this.performRefresh(), msUntilRefresh);
      if (isDevMode())
        console.log(`[AuthService] Refresh en ${Math.round(msUntilRefresh / 1000)}s`);
    } catch {
      /* token inválido, no programar */
    }
  }

  private async performRefresh(): Promise<void> {
    if (this.refreshInProgress) {
      await firstValueFrom(this.refreshQueue);
      return;
    }
    this.refreshInProgress = true;
    try {
      const res = await firstValueFrom(
        this.http.post<ApiLoginResponse>(`${this.apiBaseUrl}/auth/refresh`, {}),
      );
      this._token = res.token;
      this.scheduleProactiveRefresh();
      this.refreshQueue.next(true);
    } catch {
      if (isDevMode()) console.warn('[AuthService] Refresh fallido — cerrando sesión');
      this.logout();
      this.refreshQueue.next(false);
    } finally {
      this.refreshInProgress = false;
    }
  }

  private cancelRefreshTimer(): void {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  private checkStoredAuth(): void {
    const storedToken = sessionStorage.getItem('pf_token') ?? localStorage.getItem('pf_token');
    const storedUser = sessionStorage.getItem('currentUser') ?? localStorage.getItem('currentUser');
    if (storedToken && storedUser && !this.isTokenExpired(storedToken)) {
      try {
        this._token = storedToken;
        this.currentUserSubject.next(JSON.parse(storedUser) as User);
        this.isAuthenticatedSubject.next(true);
        this.scheduleProactiveRefresh();
      } catch {
        this.clearAuth();
      }
    } else {
      this.clearAuth();
    }
  }

  private clearAuth(): void {
    this._token = null;
    this.cancelRefreshTimer();
    sessionStorage.removeItem('pf_token');
    localStorage.removeItem('pf_token');
    localStorage.removeItem('currentUser');
    localStorage.removeItem(AuthService.SESSION_KEY);
    sessionStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private setupStorageListener(): void {
    if (this.storageListenerRegistered) return;
    this.storageListenerRegistered = true;

    globalThis.addEventListener('storage', (event) => {
      // Otra pestaña hizo logout → sincronizar
      if (event.key === AuthService.SESSION_KEY && event.newValue === null) {
        this._token = null;
        this.cancelRefreshTimer();
        this.currentUserSubject.next(null);
        this.isAuthenticatedSubject.next(false);
        this.router.navigate(['/auth/login']);
      } else if (event.key === 'pf_token' && event.newValue) {
        this._token = event.newValue;
        this.scheduleProactiveRefresh();
      }
    });
  }
}
