import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OKTA_AUTH } from '@okta/okta-angular';
import OktaAuth from '@okta/okta-auth-js';

@Component({
  selector: 'app-logout-callback',
  standalone: true,
  template: `
    <div class="d-flex flex-column align-items-center justify-content-center vh-100 text-center px-3">
      <div class="spinner-border text-primary mb-3" role="status">
        <span class="visually-hidden">Cerrando sesión...</span>
      </div>
      <p class="text-muted">Cerrando sesión...</p>
    </div>
  `,
})
export class LogoutCallbackComponent implements OnInit {
  private readonly oktaAuth = inject<OktaAuth>(OKTA_AUTH);
  private readonly router   = inject(Router);

  async ngOnInit(): Promise<void> {
    await this.clearSession();
    await this.router.navigate(['/auth/login'], { replaceUrl: true });
  }

  private async clearSession(): Promise<void> {
    try {
      await this.oktaAuth.revokeAccessToken();
    } catch {
      // token ya expirado o inexistente — continuar de todas formas
    }

    try {
      await this.oktaAuth.revokeRefreshToken();
    } catch { /* idem */ }

    try {
      this.oktaAuth.tokenManager.clear();
    } catch { /* idem */ }

    try {
      await this.oktaAuth.closeSession();
    } catch { /* idem */ }

    try { localStorage.clear(); }   catch { /* idem */ }
    try { sessionStorage.clear(); } catch { /* idem */ }
  }
}
