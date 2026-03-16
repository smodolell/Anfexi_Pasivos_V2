import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { EMPTY, catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UtilsService } from '../services/utils.service';
import { Router } from '@angular/router';

// Evita que múltiples peticiones simultáneas con 401 disparen varias notificaciones/logout
let handlingUnauthorized = false;

/** Retorna true si el error HTTP ya fue notificado por el interceptor global */
export function wasHandledByInterceptor(error: unknown): boolean {
  return !!(error && typeof error === 'object' && (error as any)['interceptorHandled'] === true);
}

export const AuthInterceptor: HttpInterceptorFn = (request, next) => {
  const authService  = inject(AuthService);
  const utilsService = inject(UtilsService);
  const router       = inject(Router);

  const token = authService.getAuthToken();
  if (token) {
    request = request.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        // Sin red: el componente tiene contexto y muestra mensaje adecuado
        return throwError(() => error);
      }

      if (error.status === 401) {
        if (!handlingUnauthorized) {
          handlingUnauthorized = true;
          utilsService.showNotification(
            'Sesión expirada',
            'Tu sesión ha expirado. Por favor inicia sesión nuevamente.',
            'warning',
          );
          setTimeout(() => {
            authService.logout();
            handlingUnauthorized = false;
          }, 1500);
        }
        // EMPTY: la página navega a /login, el callback error() del componente no se ejecuta
        return EMPTY;
      }

      if (error.status === 403) {
        utilsService.showNotification(
          'Sin permisos',
          'No tienes permisos para realizar esta acción.',
          'error',
        );
        router.navigate(['/unauthorized']);
        return EMPTY;
      }

      if (error.status >= 500) {
        utilsService.showNotification(
          'Error del servidor',
          'Ocurrió un error inesperado en el servidor. Intenta de nuevo más tarde.',
          'error',
        );
        // Re-lanza marcado para que el componente actualice su estado de carga
        // sin mostrar una segunda notificación
        return throwError(() => Object.assign(error, { interceptorHandled: true }));
      }

      return throwError(() => error);
    }),
  );
};
