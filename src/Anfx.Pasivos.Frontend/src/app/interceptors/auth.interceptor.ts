import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { EMPTY, catchError, retry, switchMap, throwError, timer } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UtilsService } from '../services/utils.service';
import { Router } from '@angular/router';

/** Header que suprime el toast automático del interceptor para una petición específica */
export const SKIP_ERROR_TOAST_HEADER = 'X-Skip-Error-Toast';

/** Retorna true si el error HTTP ya fue notificado por el interceptor global */
export function wasHandledByInterceptor(error: unknown): boolean {
  return (
    error !== null &&
    typeof error === 'object' &&
    'interceptorHandled' in error &&
    (error as Record<string, unknown>)['interceptorHandled'] === true
  );
}

type HandledHttpError = HttpErrorResponse & { interceptorHandled: true };

/** Errores transitorios que se reintentan (solo en GET): sin conexión o servicio no disponible */
const RETRYABLE_STATUSES = new Set([0, 503]);
const MAX_RETRIES = 2;

function markAsHandled(error: HttpErrorResponse): HandledHttpError {
  return Object.assign(error, { interceptorHandled: true as const });
}

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
    // Retry con backoff exponencial solo para GET y errores transitorios (red/503)
    retry({
      count: MAX_RETRIES,
      delay: (error: HttpErrorResponse, attempt) =>
        request.method === 'GET' && RETRYABLE_STATUSES.has(error.status)
          ? timer(attempt * 1000)
          : throwError(() => error),
    }),

    catchError((error: HttpErrorResponse) => {

      // CASO 0: Error de red (sin conexión) — ya se reintentó, propagar al componente
      if (error.status === 0) {
        return throwError(() => error);
      }

      // CASO 401: Intentar refresh y reintentar la petición original.
      // refreshToken() implementa single-flight: llamadas concurrentes comparten
      // el mismo HTTP request; no se necesita lógica de coordinación aquí.
      if (error.status === 401) {
        return authService.refreshToken().pipe(
          switchMap((success) => {
            if (!success) return EMPTY;
            const retried = outgoing.clone({
              setHeaders: { Authorization: `Bearer ${authService.getAuthToken()!}` },
            });
            return next(retried);
          }),
          catchError(() => EMPTY),
        );
      }

      // CASO 403: Sin permisos — notificar y navegar; componente recibe error marcado
      if (error.status === 403) {
        if (!skipToast) {
          utilsService.showNotification(
            'Sin permisos',
            'No tienes permisos para realizar esta acción.',
            'error',
          );
        }
        router.navigate(['/unauthorized']);
        return throwError(() => markAsHandled(error));
      }

      // CASO 500+: Error de servidor — notificar; componente recibe error marcado
      if (error.status >= 500) {
        if (!skipToast) {
          utilsService.showNotification(
            'Error del servidor',
            'Ocurrió un error inesperado en el servidor. Intenta de nuevo más tarde.',
            'error',
          );
        }
        return throwError(() => markAsHandled(error));
      }

      // CASO 400, 404, etc.: el componente decide cómo mostrarlo
      return throwError(() => error);
    }),
  );
};
