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

  // 1. Inyección de Token
  const token = authService.getAuthToken();
  if (token) {
    request = request.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {

      // CASO 0: Error de red (Sin conexión)
      if (error.status === 0) {
        // No lo marcamos como manejado para que el componente muestre "Error de conexión"
        return throwError(() => error);
      }

      // CASO 401: Sesión expirada
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
        return EMPTY; // Corta el flujo, no llega al componente
      }

      // CASO 403: Prohibido / Sin permisos
      if (error.status === 403) {
        utilsService.showNotification(
          'Sin permisos',
          'No tienes permisos para realizar esta acción.',
          'error',
        );
        router.navigate(['/unauthorized']);
        return EMPTY; // Corta el flujo
      }

      // CASO 500+: Errores críticos de Servidor
      if (error.status >= 500) {
        utilsService.showNotification(
          'Error del servidor',
          'Ocurrió un error inesperado en el servidor. Intenta de nuevo más tarde.',
          'error',
        );

        // --- MODIFICACIÓN CLAVE ---
        // Marcamos el error como manejado para que el componente sepa que ya hubo una notificación
        const handledError = Object.assign(error, { interceptorHandled: true });
        return throwError(() => handledError);
      }

      // CASO 400, 404, etc: Errores de validación o lógica de negocio
      // No agregamos 'interceptorHandled'.
      // Esto permite que 'wasHandledByInterceptor(err)' sea FALSE en el componente.
      return throwError(() => error);
    }),
  );
};
