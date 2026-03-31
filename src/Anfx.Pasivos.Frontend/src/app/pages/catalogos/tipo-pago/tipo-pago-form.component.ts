import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TipoPagoListItemDto, CreateTipoPagoDto, UpdateTipoPagoDto } from '../../../../types/catalogos/tipo-pago.dto';
import { CardComponent } from '@shared/components/card/card.component';

@Component({
  selector: 'app-tipo-pago-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CardComponent],
  templateUrl: './tipo-pago-form.component.html'
})
export class TipoPagoFormComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);

  @Input() tipoPago: Partial<TipoPagoListItemDto> = { tipoPago: '' };

  @Output() guardar  = new EventEmitter<CreateTipoPagoDto | UpdateTipoPagoDto>();
  @Output() cancelar = new EventEmitter<void>();

  form!: FormGroup;

  ngOnInit() {
    this.form = this.fb.group({
      tipoPago: [this.tipoPago.tipoPago || '', [Validators.required, Validators.maxLength(150)]]
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.form && changes['tipoPago']) {
      this.form.patchValue({ tipoPago: this.tipoPago.tipoPago || '' });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      this.guardar.emit({ ...this.tipoPago, ...this.form.value });
    } else {
      Object.values(this.form.controls).forEach(c => c.markAsTouched());
    }
  }

  isInvalid(field: string): boolean {
    const c = this.form.get(field);
    return !!(c && c.invalid && (c.dirty || c.touched));
  }

  getError(field: string): string {
    const c = this.form.get(field);
    if (c?.errors) {
      if (c.errors['required'])   return 'Este campo es requerido';
      if (c.errors['maxlength'])  return `Máximo ${c.errors['maxlength'].requiredLength} caracteres`;
    }
    return '';
  }
}
