import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TipoDireccionDto, CreateTipoDireccionDto, UpdateTipoDireccionDto } from '../../../../types/catalogos/tipodireccion.dto';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-tipodireccion-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tipodireccion-form.component.html'
})
export class TipoDireccionFormComponent implements OnInit, OnChanges {
  private utilsService = inject(UtilsService);
  private fb = inject(FormBuilder);

  @Input() tipoDireccion: Partial<TipoDireccionDto> = {
    sTipoDireccion: ''
  };

  @Output() guardar = new EventEmitter<CreateTipoDireccionDto | UpdateTipoDireccionDto>();
  @Output() cancelar = new EventEmitter<void>();
  @Output() volverALista = new EventEmitter<void>();

  // Reactive Form
  tipoDireccionForm!: FormGroup;

  constructor() { }

  ngOnInit() {
    // Inicializar el formulario reactivo
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    // Si el formulario ya existe y cambió el input tipoDireccion, actualizar los valores
    if (this.tipoDireccionForm && changes['tipoDireccion']) {
      this.updateFormValues();
    }
  }

  private initForm() {
    this.tipoDireccionForm = this.fb.group({
      sTipoDireccion: [this.tipoDireccion.sTipoDireccion || '', [Validators.required, Validators.minLength(2)]]
    });
  }

  private updateFormValues() {
    this.tipoDireccionForm.patchValue({
      sTipoDireccion: this.tipoDireccion.sTipoDireccion || ''
    });
  }

  onSubmit() {
    if (this.tipoDireccionForm.valid) {
      const formValue = this.tipoDireccionForm.value;
      
      // Crear objeto tipo de dirección con los valores del formulario
      const tipoDireccionData = {
        ...this.tipoDireccion,
        ...formValue
      };

      this.guardar.emit(tipoDireccionData as CreateTipoDireccionDto | UpdateTipoDireccionDto);
    } else {
      // Marcar todos los campos como touched para mostrar errores
      this.markFormGroupTouched();
    }
  }

  // Método para marcar todos los campos como touched
  private markFormGroupTouched() {
    Object.keys(this.tipoDireccionForm.controls).forEach(key => {
      const control = this.tipoDireccionForm.get(key);
      control?.markAsTouched();
    });
  }

  // Métodos helper para el template
  isFieldInvalid(fieldName: string): boolean {
    const field = this.tipoDireccionForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.tipoDireccionForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) return 'Este campo es requerido';
      if (field.errors['minlength']) return `Mínimo ${field.errors['minlength'].requiredLength} caracteres`;
    }
    return '';
  }

  onCancel() {
    this.cancelar.emit();
  }
}
