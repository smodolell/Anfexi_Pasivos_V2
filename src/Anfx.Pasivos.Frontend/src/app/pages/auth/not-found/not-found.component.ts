import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  template: `
    <div
      class="d-flex flex-column align-items-center justify-content-center vh-100 text-center px-3"
    >
      <h1 class="display-1 fw-bold text-secondary">404</h1>
      <h2 class="mb-3">Página no encontrada</h2>
      <p class="text-muted mb-4">La ruta que buscas no existe o fue removida.</p>
      <button class="btn btn-primary" (click)="router.navigate(['/admin/reportes/dashboard'])">
        Volver al inicio
      </button>
    </div>
  `,
})
export class NotFoundComponent {
  router = inject(Router);
}
