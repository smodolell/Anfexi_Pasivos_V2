import { Injectable, Inject, isDevMode } from '@angular/core';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { API_AUTH_URL } from '../api.config';

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

type LoginResult = { success: boolean; message: string; user?: User; errors?: string[] };
type ApiLoginResponse = { token: string; user: User; message?: string; errors?: string[] };

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject     = new BehaviorSubject<User | null>(null);
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  private storageListenerRegistered = false;

  readonly currentUser$     = this.currentUserSubject.asObservable();
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private readonly router: Router,
    private readonly http: HttpClient,
    @Inject(API_AUTH_URL) private readonly apiBaseUrl: string,
  ) {
    this.checkStoredAuth();
  }

  // ── Inicialización ────────────────────────────────────────────

  private checkStoredAuth(): void {
    const storedUser  = localStorage.getItem('currentUser');
    const storedToken = localStorage.getItem('authToken');

    if (storedUser && storedToken) {
      try {
        if (this.isTokenExpired(storedToken)) {
          this.clearAuth();
          return;
        }
        const user = JSON.parse(storedUser) as User;
        this.currentUserSubject.next(user);
        this.isAuthenticatedSubject.next(true);
      } catch {
        this.clearAuth();
      }
    }

    this.setupStorageListener();
  }

  // ── Login ─────────────────────────────────────────────────────

  async login(credentials: LoginCredentials): Promise<LoginResult> {
    try {
      const res = await firstValueFrom(
        this.http.post<ApiLoginResponse>(`${this.apiBaseUrl}/auth/login`, credentials),
      );

      this.clearAuth();
      localStorage.setItem('currentUser', JSON.stringify(res.user));
      localStorage.setItem('authToken', res.token);

      this.currentUserSubject.next(res.user);
      this.isAuthenticatedSubject.next(true);

      return { success: true, message: res.message ?? 'Login exitoso', user: res.user };
    } catch (err) {
      const error = err as HttpErrorResponse;

      // status 0 → sin red; cualquier otro → error del servidor
      const message = error.status === 0
        ? 'No se pudo conectar con el servidor. Verifique su conexión a internet.'
        : error.error?.message ?? error.statusText ?? 'Error en el servidor';

      const errors: string[] = error.error?.errors ?? [];
      return { success: false, message, errors };
    }
  }

  // ── Logout ────────────────────────────────────────────────────

  logout(): void {
    this.notifyOtherTabs();
    this.clearAuth();
    this.router.navigate(['/auth/login']);
  }

  // ── Estado ────────────────────────────────────────────────────

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

  // ── Token ─────────────────────────────────────────────────────

  getAuthToken(): string | null {
    return localStorage.getItem('authToken');
  }

  isTokenExpired(token?: string): boolean {
    const t = token ?? this.getAuthToken();
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

  // ── Guard helper ──────────────────────────────────────────────

  async checkAuthentication(): Promise<boolean> {
    const token = this.getAuthToken();
    if (!token || this.isTokenExpired(token)) {
      this.clearAuth();
      return false;
    }
    if (!this.isAuthenticated()) {
      this.checkStoredAuth();
    }
    return this.isAuthenticated();
  }

  // ── Internos ──────────────────────────────────────────────────

  private clearAuth(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('authToken');
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private setupStorageListener(): void {
    if (this.storageListenerRegistered) return;
    this.storageListenerRegistered = true;

    window.addEventListener('storage', (event) => {
      if (event.key !== 'currentUser' && event.key !== 'authToken') return;

      if (event.newValue === null) {
        this.clearAuth();
        this.router.navigate(['/auth/login']);
      } else if (event.key === 'currentUser') {
        try {
          const user = JSON.parse(event.newValue) as User;
          this.currentUserSubject.next(user);
          this.isAuthenticatedSubject.next(true);
        } catch {
          if (isDevMode()) console.error('[AuthService] Error al sincronizar usuario entre pestañas');
        }
      }
    });
  }

  private notifyOtherTabs(): void {
    // El propio evento 'storage' que dispara clearAuth() (removeItem)
    // es suficiente para notificar otras pestañas; no se necesita señal adicional
    localStorage.setItem('auth_logout', Date.now().toString());
    localStorage.removeItem('auth_logout');
  }
}
