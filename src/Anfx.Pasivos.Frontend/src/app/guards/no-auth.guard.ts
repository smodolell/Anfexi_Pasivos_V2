import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Impide que un usuario ya autenticado acceda a rutas de login */
export const noAuthGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  return authService.isAuthenticated()
    ? router.createUrlTree(['/admin'])
    : true;
};
