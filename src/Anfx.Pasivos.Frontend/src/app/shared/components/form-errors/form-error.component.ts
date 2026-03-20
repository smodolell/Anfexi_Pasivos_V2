import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-form-errors',
  standalone: true,
  template: `
    @if (errors().length > 0) {
      <div class="alert alert-danger alert-dismissible fade show my-3" role="alert">
        <div class="d-flex align-items-center">
          <i class="bi bi-exclamation-triangle-fill me-2"></i>
          <div>
            <ul class="mb-0 ps-3">
              @for (error of errors(); track $index) {
                <li>{{ error }}</li>
              }
            </ul>
          </div>
        </div>
        <button type="button" class="btn-close" (click)="onClose.emit()" aria-label="Close"></button>
      </div>
    }
  `,
  styles: [`
    .alert ul { list-style-type: disc; }
    .alert { border-left: 5px solid #dc3545; }
  `]
})
export class FormErrorsComponent {
  // Recibe la lista de errores (funciona perfecto con tus Signals)
  errors = input<string[]>([]);

  // Evento opcional para cuando el usuario cierra la alerta manualmente
  onClose = output<void>();
}
