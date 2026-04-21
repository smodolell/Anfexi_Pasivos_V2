import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OktaAuthStateService } from '@okta/okta-angular';
import { filter, map, take } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const oktaState = inject(OktaAuthStateService);
  const router    = inject(Router);

  return oktaState.authState$.pipe(
    filter(state => !!state && state.isAuthenticated !== undefined),
    take(1),
    map(state => state.isAuthenticated ? true : router.createUrlTree(['/auth/login'])),
  );
};
