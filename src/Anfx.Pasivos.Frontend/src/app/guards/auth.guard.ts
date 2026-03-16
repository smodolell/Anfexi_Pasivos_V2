import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { from, map } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  // Verificación síncrona: ya autenticado en memoria
  if (authService.isAuthenticated()) {
    return true;
  }

  // Sin token → al login de inmediato
  if (!authService.getAuthToken()) {
    return router.createUrlTree(['/auth/login']);
  }

  // Token presente pero estado en memoria vacío (ej. recarga de página)
  // → verificar validez y restaurar sesión
  return from(authService.checkAuthentication()).pipe(
    map(ok => ok || router.createUrlTree(['/auth/login'])),
  );
};
