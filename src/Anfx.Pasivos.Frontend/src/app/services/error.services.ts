import { Injectable, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService {
  // Signal centralizado
  private errorsSignal = signal<string[]>([]);
  public readonly errors = this.errorsSignal.asReadonly();

  public setErrors(err: unknown) {
    this.errorsSignal.set(this.parseError(err));
  }

  public clear() {
    this.errorsSignal.set([]);
  }
  /** Extrae los mensajes de error del ApiResponseDto de .NET */
  public parseError(err: unknown): string[] {
    if (!(err instanceof HttpErrorResponse)) {
      return ['Ocurrió un error inesperado'];
    }

    // Estructura de tu ApiResponseDto de C#
    const apiResponse = err.error;
    const errors: string[] = apiResponse?.errors?.filter((e: any) => !!e) || [];

    if (errors.length > 0) return errors;

    return [apiResponse?.message || 'Error de comunicación con el servidor'];
  }
}
