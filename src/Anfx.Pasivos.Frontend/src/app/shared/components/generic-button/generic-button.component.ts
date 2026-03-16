import {
  Component, Input, Output, EventEmitter, ChangeDetectionStrategy
} from '@angular/core';
import { CommonModule } from '@angular/common';

export type ButtonVariant = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light';
export type ButtonSize    = 'sm' | 'md' | 'lg';
export type ButtonType    = 'button' | 'submit' | 'reset';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './generic-button.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GenericButtonComponent {
  /** Clase FontAwesome, ej: 'fa-solid fa-plus' */
  @Input() icon: string | null = null;
  /** Texto del botón */
  @Input() label: string | null = null;
  /** Variante de color Bootstrap */
  @Input() variant: ButtonVariant = 'primary';
  /** Tamaño */
  @Input() size: ButtonSize = 'md';
  /** Tipo HTML del botón */
  @Input() type: ButtonType = 'button';
  /** Deshabilita el botón */
  @Input() disabled = false;
  /** Muestra spinner y deshabilita mientras está activo */
  @Input() loading = false;
  /** Clases CSS adicionales */
  @Input() extraClass = '';

  @Output() clicked = new EventEmitter<MouseEvent>();

  get btnClass(): string {
    const size = this.size === 'md' ? '' : `btn-${this.size}`;
    return ['btn', `btn-${this.variant}`, size, this.extraClass]
      .filter(Boolean)
      .join(' ');
  }

  onClick(event: MouseEvent) {
    if (!this.disabled && !this.loading) {
      this.clicked.emit(event);
    }
  }
}
