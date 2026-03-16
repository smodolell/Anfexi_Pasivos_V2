import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  template: `
    <div class="d-flex flex-column align-items-center justify-content-center vh-100 text-center px-3">
      <h1 class="display-1 fw-bold text-danger">403</h1>
      <h2 class="mb-3">Acceso denegado</h2>
      <p class="text-muted mb-4">No tienes permisos para acceder a esta sección.</p>
      <button class="btn btn-primary" (click)="router.navigate(['/admin/reportes/dashboard'])">
        Volver al inicio
      </button>
    </div>
  `,
})
export class UnauthorizedComponent {
  router = inject(Router);
}
