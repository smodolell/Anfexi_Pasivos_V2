import { Component, OnInit, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RolDto, CreateRolDto, UpdateRolDto } from '../../../../types/sistema/rol.dto';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-rol-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './rol-form.component.html'
})
export class RolFormComponent implements OnInit {
  @Input() rol: Partial<RolDto> = {};
  @Output() guardar = new EventEmitter<CreateRolDto | UpdateRolDto>();
  @Output() cancelar = new EventEmitter<void>();
  @Output() volverALista = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private utilsService = inject(UtilsService);

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      sRol: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    if (this.rol) {
      this.form.patchValue({
        sRol: this.rol.sRol,
        descripcion: this.rol.descripcion
      });
    }
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formData = this.form.value;
      
      if (this.rol) {
        // Actualizar
        const updateData: UpdateRolDto = {
          ...this.rol,
          ...formData
        };
        this.guardar.emit(updateData);
      } else {
        // Crear
        const createData: CreateRolDto = {
          ...formData
        };
        this.guardar.emit(createData);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  onCancel(): void {
    this.cancelar.emit();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.form.controls).forEach(key => {
      const control = this.form.get(key);
      if (control) {
        control.markAsTouched();
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) {
        return 'Este campo es requerido';
      }
      if (field.errors['maxlength']) {
        return `Máximo ${field.errors['maxlength'].requiredLength} caracteres`;
      }
    }
    return '';
  }
}
