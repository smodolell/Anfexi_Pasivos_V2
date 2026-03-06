import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ColoniaDto, CreateColoniaDto, UpdateColoniaDto } from '../../../../types/catalogos/colonia.dto';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-colonia-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './colonia-form.component.html'
})
export class ColoniaFormComponent implements OnInit, OnChanges {
  private utilsService = inject(UtilsService);
  private fb = inject(FormBuilder);

  @Input() colonia: Partial<ColoniaDto> = {
    sColonia: '',
    estado: '',
    municipio: '',
    codigoPostal: ''
  };

  @Output() guardar = new EventEmitter<CreateColoniaDto | UpdateColoniaDto>();
  @Output() cancelar = new EventEmitter<void>();
  @Output() volverALista = new EventEmitter<void>();

  // Reactive Form
  coloniaForm!: FormGroup;

  constructor() { }

  ngOnInit() {
    // Inicializar el formulario reactivo
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    // Si el formulario ya existe y cambió el input colonia, actualizar los valores
    if (this.coloniaForm && changes['colonia']) {
      this.updateFormValues();
    }
  }

  private initForm() {
    this.coloniaForm = this.fb.group({
      sColonia: [this.colonia.sColonia || '', [Validators.required, Validators.minLength(2)]],
      estado: [this.colonia.estado || '', [Validators.required, Validators.minLength(2)]],
      municipio: [this.colonia.municipio || '', [Validators.required, Validators.minLength(2)]],
      codigoPostal: [this.colonia.codigoPostal || '', [Validators.required, Validators.pattern(/^\d{5}$/)]]
    });
  }

  private updateFormValues() {
    this.coloniaForm.patchValue({
      sColonia: this.colonia.sColonia || '',
      estado: this.colonia.estado || '',
      municipio: this.colonia.municipio || '',
      codigoPostal: this.colonia.codigoPostal || ''
    });
  }

  onSubmit() {
    if (this.coloniaForm.valid) {
      const formValue = this.coloniaForm.value;
      
      // Crear objeto colonia con los valores del formulario
      const coloniaData = {
        ...this.colonia,
        ...formValue
      };

      this.guardar.emit(coloniaData as CreateColoniaDto | UpdateColoniaDto);
    } else {
      // Marcar todos los campos como touched para mostrar errores
      this.markFormGroupTouched();
    }
  }

  // Método para marcar todos los campos como touched
  private markFormGroupTouched() {
    Object.keys(this.coloniaForm.controls).forEach(key => {
      const control = this.coloniaForm.get(key);
      control?.markAsTouched();
    });
  }

  // Métodos helper para el template
  isFieldInvalid(fieldName: string): boolean {
    const field = this.coloniaForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.coloniaForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) return 'Este campo es requerido';
      if (field.errors['minlength']) return `Mínimo ${field.errors['minlength'].requiredLength} caracteres`;
      if (field.errors['pattern']) return 'El código postal debe tener 5 dígitos';
    }
    return '';
  }

  onCancel() {
    this.cancelar.emit();
  }
}
