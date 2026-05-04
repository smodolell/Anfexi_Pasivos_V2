import { Injectable, Inject, isDevMode } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  TimeoutError,
  catchError,
  finalize,
  firstValueFrom,
  map,
  of,
  shareReplay,
  timeout,
} from 'rxjs';
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
const HTTP_TIMEOUT_MS = 30_000;

@Injectable({ providedIn: 'root' })
export class AuthService {
  // ── Estado ────────────────────────────────────────────────
  private readonly currentUserSubject = new BehaviorSubject<User | null>(null);
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  readonly currentUser$ = this.currentUserSubject.asObservable();
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  // ── Token en memoria ──────────────────────────────────────
  private _token: string | null = null;

  // ── Refresh (single-flight) ───────────────────────────────
  private refreshTimer: ReturnType<typeof setTimeout> | null = null;

  /**
   * Cuando es distinto de null hay un HTTP de refresh en vuelo.
   * Todos los callers concurrentes reciben la MISMA referencia y comparten
   * el único request gracias a shareReplay(1).
   *
   * finalize() se coloca ANTES de shareReplay para que se ejecute exactamente
   * una vez —cuando el HTTP completa o falla— y no una vez por suscriptor.
   */
  private refreshInFlight$: Observable<boolean> | null = null;

  // ── Cross-tab logout ──────────────────────────────────────
  private storageListenerRegistered = false;
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
        this.http
          .post<ApiLoginResponse>(`${this.apiBaseUrl}/auth/login`, credentials)
          .pipe(timeout(HTTP_TIMEOUT_MS)),
      );

      const storage = credentials.recuerdame ? localStorage : sessionStorage;
      this.commitSession(res.token, res.user, storage);
      localStorage.setItem(AuthService.SESSION_KEY, '1');
      this.scheduleProactiveRefresh();

      return { success: true, message: res.message ?? 'Login exitoso', user: res.user };
    } catch (err) {
      if (err instanceof TimeoutError) {
        return {
          success: false,
          message: 'La solicitud tardó demasiado. Intente nuevamente.',
          errors: [],
        };
      }
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
    await firstValueFrom(
      this.http
        .post(`${this.apiBaseUrl}/auth/recovery`, { email })
        .pipe(timeout(HTTP_TIMEOUT_MS)),
    );
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
    this.clearAuth();
    return false;
  }

  // ── Refresh (single-flight) ───────────────────────────────

  /**
   * Inicia o reutiliza el refresh HTTP en curso.
   *
   * Primera llamada: crea el Observable, lo asigna a refreshInFlight$ y lo retorna.
   * Llamadas concurrentes: devuelven la misma referencia; shareReplay(1) distribuye
   * el resultado a todos los suscriptores sin lanzar un segundo HTTP.
   *
   * Emite true si el refresh fue exitoso; false si falló (sesión cerrada internamente).
   * El interceptor usa switchMap sobre este Observable para reintentar la request.
   */
  refreshToken(): Observable<boolean> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    this.refreshInFlight$ = this.http
      .post<ApiLoginResponse>(`${this.apiBaseUrl}/auth/refresh`, {})
      .pipe(
        timeout(HTTP_TIMEOUT_MS),
        map((res) => {
          this.commitTokenRefresh(res.token);
          return true;
        }),
        catchError(() => {
          if (isDevMode()) console.warn('[AuthService] Refresh fallido — cerrando sesión');
          this.clearAuth();
          this.router.navigate(['/auth/login']);
          return of(false);
        }),
        // finalize ANTES de shareReplay: se ejecuta una sola vez cuando la fuente
        // HTTP completa, no una vez por suscriptor al desuscribirse.
        finalize(() => {
          this.refreshInFlight$ = null;
        }),
        // refCount: false (default en shareReplay(1)) mantiene la suscripción
        // interna activa aunque un caller se desuscriba antes de que otros terminen,
        // garantizando que todos reciban el resultado aunque lleguen tarde.
        shareReplay(1),
      );

    return this.refreshInFlight$;
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
        void firstValueFrom(this.refreshToken());
        return;
      }

      this.refreshTimer = setTimeout(
        () => void firstValueFrom(this.refreshToken()),
        msUntilRefresh,
      );

      if (isDevMode()) {
        console.log(`[AuthService] Refresh en ${Math.round(msUntilRefresh / 1000)}s`);
      }
    } catch {
      /* token inválido, no programar */
    }
  }

  private cancelRefreshTimer(): void {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  // ── Sesión ────────────────────────────────────────────────

  private commitSession(token: string, user: User, storage: Storage): void {
    this._token = token;
    storage.setItem('pf_token', token);
    storage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
    this.isAuthenticatedSubject.next(true);
  }

  /**
   * Actualiza el token en memoria Y en el mismo storage donde vive la sesión activa,
   * evitando el estado inconsistente de token nuevo en memoria pero obsoleto en storage.
   */
  private commitTokenRefresh(token: string): void {
    this._token = token;
    const storage = sessionStorage.getItem('pf_token') ? sessionStorage : localStorage;
    storage.setItem('pf_token', token);
    this.scheduleProactiveRefresh();
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

    globalThis.addEventListener('storage', (event: StorageEvent) => {
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
