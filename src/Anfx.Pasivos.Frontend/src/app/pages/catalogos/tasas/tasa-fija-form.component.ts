import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TasaFijaListItemDto } from '@api/models/tasaFijaListItemDto';
import { CardComponent } from '@shared/components/card/card.component';

@Component({
  selector: 'app-tasa-fija-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CardComponent],
  templateUrl: './tasa-fija-form.component.html'
})
export class TasaFijaFormComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);

  @Input() tasa: Partial<TasaFijaListItemDto> = {};

  @Output() guardar  = new EventEmitter<any>();
  @Output() cancelar = new EventEmitter<void>();

  form!: FormGroup;

  ngOnInit() {
    this.form = this.fb.group({
      nombre:    [this.tasa.nombre    || '', [Validators.required, Validators.maxLength(150)]],
      valorTasa: [this.tasa.valorTasa ?? null, [Validators.required, Validators.min(0), Validators.max(100)]],
      fecTasa:   [this.tasa.fecTasa   || null]
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.form && changes['tasa']) {
      this.form.patchValue({
        nombre:    this.tasa.nombre    || '',
        valorTasa: this.tasa.valorTasa ?? null,
        fecTasa:   this.tasa.fecTasa   || null
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      this.guardar.emit({ ...this.tasa, ...this.form.value });
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
      if (c.errors['min'])        return `El valor mínimo es ${c.errors['min'].min}`;
      if (c.errors['max'])        return `El valor máximo es ${c.errors['max'].max}`;
    }
    return '';
  }
}
