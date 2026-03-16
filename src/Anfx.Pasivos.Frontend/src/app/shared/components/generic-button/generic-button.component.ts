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
  styles: [`
    :host { display: inline-flex; }

    button {
      transition: transform .15s ease, box-shadow .15s ease;

      &:not(:disabled):hover {
        transform: translateY(-1px);
      }

      &:not(:disabled):active {
        transform: translateY(0);
        box-shadow: none !important;
      }

      /* Glow por variante — mismo estilo que search input y pill filter */
      &.btn-primary:not(:disabled):hover   { box-shadow: 0 0 0 3px rgba(29,  108, 245, .20); }
      &.btn-success:not(:disabled):hover   { box-shadow: 0 0 0 3px rgba(39,  174,  96, .20); }
      &.btn-info:not(:disabled):hover      { box-shadow: 0 0 0 3px rgba(41,  128, 185, .20); }
      &.btn-danger:not(:disabled):hover    { box-shadow: 0 0 0 3px rgba(231,  76,  60, .20); }
      &.btn-warning:not(:disabled):hover   { box-shadow: 0 0 0 3px rgba(243, 156,  18, .20); }
      &.btn-secondary:not(:disabled):hover { box-shadow: 0 0 0 3px rgba(127, 140, 141, .20); }
      &.btn-light:not(:disabled):hover     { box-shadow: 0 0 0 3px rgba(  0,   0,   0, .08); }
    }
  `],
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
