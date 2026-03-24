import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { EMPTY, catchError, retry, throwError, timer } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UtilsService } from '../services/utils.service';
import { Router } from '@angular/router';

/** Header que suprime el toast automático del interceptor para una petición específica */
export const SKIP_ERROR_TOAST_HEADER = 'X-Skip-Error-Toast';

/** Retorna true si el error HTTP ya fue notificado por el interceptor global */
export function wasHandledByInterceptor(error: unknown): boolean {
  return !!(error && typeof error === 'object' && (error as any)['interceptorHandled'] === true);
}

// Evita que múltiples peticiones simultáneas con 401 disparen varias notificaciones/logout
let handlingUnauthorized = false;

/** Errores transitorios que se reintentan: sin conexión o servicio temporalmente no disponible */
const RETRYABLE_STATUSES = new Set([0, 503]);
const MAX_RETRIES = 2;

export const AuthInterceptor: HttpInterceptorFn = (request, next) => {
  const authService  = inject(AuthService);
  const utilsService = inject(UtilsService);
  const router       = inject(Router);

  const skipToast = request.headers.has(SKIP_ERROR_TOAST_HEADER);

  // Clonar quitando el header interno antes de enviar al servidor
  const outgoing = skipToast
    ? request.clone({ headers: request.headers.delete(SKIP_ERROR_TOAST_HEADER) })
    : request;

  // Inyección de Token
  const token = authService.getAuthToken();
  const withAuth = token
    ? outgoing.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : outgoing;

  return next(withAuth).pipe(
    // Retry con backoff exponencial solo para errores transitorios (red/503)
    retry({
      count: MAX_RETRIES,
      delay: (error: HttpErrorResponse, attempt) =>
        RETRYABLE_STATUSES.has(error.status) ? timer(attempt * 1000) : throwError(() => error),
    }),

    catchError((error: HttpErrorResponse) => {

      // CASO 0: Error de red (sin conexión) — ya se reintentó, no toast automático
      if (error.status === 0) {
        return throwError(() => error);
      }

      // CASO 401: Sesión expirada
      if (error.status === 401) {
        if (!handlingUnauthorized) {
          handlingUnauthorized = true;
          if (!skipToast) {
            utilsService.showNotification(
              'Sesión expirada',
              'Tu sesión ha expirado. Por favor inicia sesión nuevamente.',
              'warning',
            );
          }
          setTimeout(() => {
            authService.logout();
            handlingUnauthorized = false;
          }, 1500);
        }
        return EMPTY;
      }

      // CASO 403: Sin permisos
      if (error.status === 403) {
        if (!skipToast) {
          utilsService.showNotification(
            'Sin permisos',
            'No tienes permisos para realizar esta acción.',
            'error',
          );
        }
        router.navigate(['/unauthorized']);
        return EMPTY;
      }

      // CASO 500+: Error de servidor
      if (error.status >= 500) {
        if (!skipToast) {
          utilsService.showNotification(
            'Error del servidor',
            'Ocurrió un error inesperado en el servidor. Intenta de nuevo más tarde.',
            'error',
          );
        }
        const handledError = Object.assign(error, { interceptorHandled: true });
        return throwError(() => handledError);
      }

      // CASO 400, 404, etc: el componente decide cómo mostrarlo
      return throwError(() => error);
    }),
  );
};
