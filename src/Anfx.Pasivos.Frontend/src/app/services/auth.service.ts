import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { OKTA_AUTH, OktaAuthStateService } from '@okta/okta-angular';
import OktaAuth, { IDToken } from '@okta/okta-auth-js';
import { environment } from 'src/environments/environment';

export interface User {
  id: number;
  nombreCompleto: string;
  email: string;
  usuarioNombre: string;
  role: 'Webmaster' | 'Admin' | 'User';
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly oktaAuth   = inject<OktaAuth>(OKTA_AUTH);
  private readonly oktaState  = inject(OktaAuthStateService);
  private readonly router     = inject(Router);

  readonly isAuthenticated$: Observable<boolean> = this.oktaState.authState$.pipe(
    map(state => state?.isAuthenticated ?? false),
  );

  readonly currentUser$: Observable<User | null> = this.oktaState.authState$.pipe(
    map(state =>
      state?.isAuthenticated && state.idToken
        ? this.claimsToUser(state.idToken)
        : null,
    ),
  );

  // ── Métodos síncronos ─────────────────────────────────────

  isAuthenticated(): boolean {
    return this.oktaAuth.authStateManager.getAuthState()?.isAuthenticated ?? false;
  }

  getCurrentUser(): User | null {
    const state = this.oktaAuth.authStateManager.getAuthState();
    return state?.isAuthenticated && state.idToken
      ? this.claimsToUser(state.idToken)
      : null;
  }

  isAdmin(): boolean {
    const role = this.getCurrentUser()?.role;
    return role === 'Admin' || role === 'Webmaster';
  }

  hasRole(role: string): boolean {
    return this.getCurrentUser()?.role === role;
  }

  /** Retorna el access token de Okta para inyectar en peticiones HTTP */
  getAuthToken(): string | undefined {
    return this.oktaAuth.getAccessToken();
  }

  // ── Flujo OAuth ───────────────────────────────────────────

  async login(): Promise<void> {
    await this.oktaAuth.signInWithRedirect({ originalUri: '/admin/reportes/dashboard' });
  }

  async logout(): Promise<void> {
    await this.oktaAuth.signOut( {
        postLogoutRedirectUri: environment.okta.logoutRedirectUri
    });
  }

  /** Verifica el estado de autenticación de forma asíncrona (usado en guards) */
  async checkAuthentication(): Promise<boolean> {
    const state = await this.oktaAuth.authStateManager.getAuthState();
    return state?.isAuthenticated ?? false;
  }

  // ── Mapeo de claims ───────────────────────────────────────

  private claimsToUser(idToken: IDToken): User {
    const claims  = idToken.claims as Record<string, unknown>;
    const groups  = (claims['groups'] as string[]) ?? [];
    return {
      id:             0,
      nombreCompleto: (claims['name'] as string) ?? String(claims['sub'] ?? ''),
      email:          (claims['email'] as string) ?? '',
      usuarioNombre:  (claims['preferred_username'] as string) ?? (claims['email'] as string) ?? '',
      role:           this.mapGroupsToRole(groups),
    };
  }

  private mapGroupsToRole(groups: string[]): 'Webmaster' | 'Admin' | 'User' {
    if (groups.includes('Webmaster')) return 'Webmaster';
    if (groups.includes('Admin'))     return 'Admin';
    return 'User';
  }
}
