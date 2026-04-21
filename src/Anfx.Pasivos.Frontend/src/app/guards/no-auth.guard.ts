import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OktaAuthStateService } from '@okta/okta-angular';
import { filter, map, take } from 'rxjs';

/** Impide que un usuario ya autenticado con Okta acceda a rutas de login */
export const noAuthGuard: CanActivateFn = () => {
  const oktaState = inject(OktaAuthStateService);
  const router    = inject(Router);

  return oktaState.authState$.pipe(
    filter(state => !!state && state.isAuthenticated !== undefined),
    take(1),
    map(state => state.isAuthenticated ? router.createUrlTree(['/admin']) : true),
  );
};
