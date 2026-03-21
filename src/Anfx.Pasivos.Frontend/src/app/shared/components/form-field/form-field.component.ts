import {
  Component, Input, Injector, forwardRef,
  OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef, inject
} from '@angular/core';
import {
  ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl, AbstractControl
} from '@angular/forms';
import { Subscription } from 'rxjs';

let _fid = 0;

/**
 * Input genérico para formularios reactivos con label, foco y validación integrados.
 *
 * Uso básico:
 *   <app-form-field formControlName="email" label="Email" type="email" [required]="true" />
 *
 * Textarea:
 *   <app-form-field formControlName="descripcion" label="Descripción" type="textarea" [rows]="4" />
 *
 * Con placeholder:
 *   <app-form-field formControlName="nombre" label="Nombre" placeholder="Ej: Juan" [required]="true" />
 */
@Component({
  selector: 'app-form-field',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormFieldComponent),
      multi: true
    }
  ],
  template: `
    <div class="pf-field">

      @if (label) {
        <label [for]="uid" class="pf-field__label">
          {{ label }}
          @if (required) {
            <span class="pf-field__required" aria-hidden="true">*</span>
          }
        </label>
      }

      @if (type === 'textarea') {
        <textarea
          [id]="uid"
          class="form-control pf-field__input"
          [class.is-invalid]="isInvalid"
          [placeholder]="placeholder"
          [rows]="rows"
          [disabled]="isDisabled"
          (input)="onInput($event)"
          (blur)="onTouched()">{{ value }}</textarea>
      } @else {
        <input
          [type]="type"
          [id]="uid"
          class="form-control pf-field__input"
          [class.is-invalid]="isInvalid"
          [value]="value"
          [placeholder]="placeholder"
          [disabled]="isDisabled"
          (input)="onInput($event)"
          (blur)="onTouched()">
      }

      @if (isInvalid) {
        <div class="pf-field__error">{{ errorMessage }}</div>
      }

    </div>
  `,
  styles: [`
    :host { display: block; }

    /* ── Contenedor del campo ─────────────────── */
    .pf-field { position: relative; }

    /* ── Label ───────────────────────────────── */
    .pf-field__label {
      display: block;
      font-weight: 600;
      font-size: 0.875rem;
      color: var(--pf-text-base);
      margin-bottom: 0.35rem;
    }

    .pf-field__required {
      color: var(--pf-danger);
      margin-left: 2px;
    }

    /* ── Input ───────────────────────────────── */
    .pf-field__input {
      border-radius: 6px;
      border: 1px solid #ced4da;
      font-size: 0.9rem;
      color: var(--pf-text-base);
      background-color: #fff;
      transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;

      &::placeholder {
        color: #9ca3af;
        font-size: 0.85rem;
      }

      /* ── Estado focus (anula Bootstrap default azul) ── */
      &:focus {
        outline: none;
        border-color: var(--pf-primary-hover);
        box-shadow: 0 0 0 0.2rem rgba(0, 75, 141, 0.15);
      }

      /* ── Estado inválido ─────────────────────── */
      &.is-invalid {
        border-color: var(--pf-danger);

        &:focus {
          border-color: var(--pf-danger);
          box-shadow: 0 0 0 0.2rem rgba(185, 28, 28, 0.15);
        }
      }

      /* ── Estado deshabilitado ────────────────── */
      &:disabled {
        background-color: var(--pf-bg-subtle);
        color: var(--pf-text-muted);
        cursor: not-allowed;
        opacity: 1;
      }
    }

    /* ── Mensaje de error ────────────────────── */
    .pf-field__error {
      font-size: 0.8rem;
      color: var(--pf-danger);
      margin-top: 0.25rem;
    }
  `]
})
export class FormFieldComponent implements ControlValueAccessor, OnInit, OnDestroy {

  /** Texto del label visible */
  @Input() label = '';

  /** Tipo de input */
  @Input() type: 'text' | 'email' | 'password' | 'number' | 'textarea' = 'text';

  /** Placeholder del campo */
  @Input() placeholder = '';

  /** Muestra asterisco (*) junto al label */
  @Input() required = false;

  /** Filas visibles (solo type="textarea") */
  @Input() rows = 3;

  /** Sobreescribe el id auto-generado */
  @Input() set inputId(v: string) { this.uid = v; }

  uid = `ff-${++_fid}`;
  value: any = '';
  isDisabled = false;

  private readonly cdr     = inject(ChangeDetectorRef);
  private readonly injector = inject(Injector);
  private sub?: Subscription;

  /** NgControl resuelto de forma lazy para evitar ciclo en DI */
  private get ngControl(): NgControl | null {
    return this.injector.get(NgControl, null, { self: true, optional: true } as any);
  }

  ngOnInit(): void {
    // Re-renderizar cuando el estado del control cambia externamente
    this.sub = this.ngControl?.control?.statusChanges.subscribe(() =>
      this.cdr.markForCheck()
    );
  }

  ngOnDestroy(): void { this.sub?.unsubscribe(); }

  // ── Acceso al AbstractControl ──────────────────────────────────────
  get control(): AbstractControl | null { return this.ngControl?.control ?? null; }

  get isInvalid(): boolean {
    const c = this.control;
    return !!(c?.invalid && (c?.touched || c?.dirty));
  }

  get errorMessage(): string {
    const errors = this.control?.errors;
    if (!errors) return '';
    if (errors['required'])     return 'Este campo es obligatorio.';
    if (errors['email'])        return 'Ingrese un correo electrónico válido.';
    if (errors['minlength'])    return `Mínimo ${errors['minlength'].requiredLength} caracteres.`;
    if (errors['maxlength'])    return `Máximo ${errors['maxlength'].requiredLength} caracteres.`;
    if (errors['min'])          return `El valor mínimo es ${errors['min'].min}.`;
    if (errors['max'])          return `El valor máximo es ${errors['max'].max}.`;
    if (errors['pattern'])      return 'Formato no válido.';
    if (errors['passwordMismatch']) return 'Las contraseñas no coinciden.';
    // Mensaje personalizado: el validador puede devolver un string directamente
    const firstKey = Object.keys(errors)[0];
    const val = errors[firstKey];
    return typeof val === 'string' ? val : 'Valor no válido.';
  }

  // ── ControlValueAccessor ───────────────────────────────────────────
  private _onChange: (v: any) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: any): void {
    this.value = value ?? '';
    this.cdr.markForCheck();
  }

  registerOnChange(fn: (v: any) => void): void { this._onChange = fn; }
  registerOnTouched(fn: () => void): void       { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
    this.cdr.markForCheck();
  }

  onInput(event: Event): void {
    const target = event.target as HTMLInputElement | HTMLTextAreaElement;
    this.value = this.type === 'number'
      ? (target.value !== '' ? Number(target.value) : null)
      : target.value;
    this._onChange(this.value);
    this.cdr.markForCheck();
  }
}
