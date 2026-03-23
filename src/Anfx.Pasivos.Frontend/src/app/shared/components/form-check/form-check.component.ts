import {
  Component, Input, forwardRef,
  ChangeDetectionStrategy, ChangeDetectorRef, inject
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

let _uid = 0;

/**
 * Componente genérico para checkboxes y toggles en formularios reactivos.
 *
 * Uso como **toggle switch** (label dinámico Activo/Inactivo):
 *   <app-form-check
 *     formControlName="activo"
 *     mode="switch"
 *     trueLabel="Activo"
 *     falseLabel="Inactivo">
 *   </app-form-check>
 *
 * Uso como **checkbox estándar** con label fijo:
 *   <app-form-check
 *     formControlName="permiteEditarContrasenia"
 *     label="Permitir al usuario cambiar su contraseña">
 *   </app-form-check>
 */
@Component({
  selector: 'app-form-check',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormCheckComponent),
      multi: true
    }
  ],
  template: `
    <div class="form-check" [class.form-switch]="mode === 'switch'" [style.--fc-color]="colorValue">
      <input
        class="form-check-input"
        type="checkbox"
        role="switch"
        [id]="uid"
        [checked]="value"
        [disabled]="isDisabled"
        (change)="onCheckChange($event)"
        (blur)="onTouched()">
      <label class="form-check-label" [for]="uid">
        @if (mode === 'switch') {
          {{ value ? trueLabel : falseLabel }}
        } @else {
          {{ label }}
        }
      </label>
    </div>
  `,
  styles: [`
    :host { display: block; }

    /* ── Color dinámico en estado checked ─── */
    .form-check-input:checked {
      background-color: var(--fc-color);
      border-color:     var(--fc-color);
    }

    /* ── Focus ring dinámico ─────────────── */
    .form-check-input:focus {
      border-color: var(--fc-color);
      box-shadow: 0 0 0 0.2rem color-mix(in srgb, var(--fc-color) 30%, transparent);
    }

    /* ── Cursor y estilo de label ───────────── */
    .form-check-input { cursor: pointer; }
    .form-check-label {
      font-size: 0.9rem;
      color: var(--color-text-primary);
      cursor: pointer;
      user-select: none;
      padding-left: 0.25em;   /* gap extra entre toggle y texto */
    }
  `]
})
export class FormCheckComponent implements ControlValueAccessor {

  /** Label estático (modo checkbox) */
  @Input() label = '';

  /** Label cuando el valor es true (modo switch) */
  @Input() trueLabel = 'Activo';

  /** Label cuando el valor es false (modo switch) */
  @Input() falseLabel = 'Inactivo';

  /** 'switch' = toggle visual,  'checkbox' = cuadro estándar */
  @Input() mode: 'switch' | 'checkbox' = 'checkbox';

  /** Color del checkbox/switch en estado checked. Por defecto: 'success' */
  @Input() color: 'primary' | 'success' | 'warning' | 'info' | 'danger' = 'success';

  /** Sobreescribe el id auto-generado si se necesita referenciar externamente */
  @Input() set inputId(v: string) { this.uid = v; }

  uid = `fc-${++_uid}`;

  get colorValue(): string {
    const map: Record<string, string> = {
      primary: 'var(--color-primary)',
      success: 'var(--color-success)',
      warning: 'var(--color-warning)',
      info:    'var(--color-info)',
      danger:  'var(--color-danger-bs)',
    };
    return map[this.color] ?? 'var(--color-success)';
  }
  value = false;
  isDisabled = false;

  private cdr = inject(ChangeDetectorRef);

  private _onChange: (v: boolean) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: boolean): void {
    this.value = !!value;
    this.cdr.markForCheck();
  }

  registerOnChange(fn: (v: boolean) => void): void { this._onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
    this.cdr.markForCheck();
  }

  onCheckChange(event: Event): void {
    this.value = (event.target as HTMLInputElement).checked;
    this._onChange(this.value);
    this.cdr.markForCheck();
  }
}
