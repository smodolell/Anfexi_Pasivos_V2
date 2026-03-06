import { Component, Input, Output, EventEmitter, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { UsuarioService } from '../../../services/sistema/usuario.service';
import { UsuarioFormData, UsuarioDto, CreateUsuarioDto, UpdateUsuarioDto, RolItemDto } from '../../../../types/sistema/usuario.dto';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-usuario-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './usuario-form.component.html'
})
export class UsuarioFormComponent implements OnInit {
  private utilsService = inject(UtilsService);
  private usuarioService = inject(UsuarioService);
  private fb = inject(FormBuilder);

  @Input() usuario: UsuarioFormData = {
    nombreCompleto: '',
    email: '',
    usuarioNombre: '',
    activo: true,
    rolId: undefined,
    Contrasenia: '',
    Confirma_Contrasenia: '',
    PermiteEditarContrasenia: false
  };

  @Input() isFromProfile: boolean = false;

  @Output() guardar = new EventEmitter<CreateUsuarioDto | UpdateUsuarioDto>();
  @Output() cancelar = new EventEmitter<void>();
  @Output() volverALista = new EventEmitter<void>();

  roles = signal<RolItemDto[]>([]);
  loadingRoles = signal<boolean>(false);

  // Reactive Form
  usuarioForm!: FormGroup;

  constructor() { }

  ngOnInit() {
    // Inicializar el formulario reactivo
    this.initForm();

    // Cargar los roles
    this.cargarRoles();
  }

  // Validador personalizado para contraseñas iguales
  private passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const contrasenia = control.get('contrasenia');
      const confirma_contrasenia = control.get('confirma_contrasenia');

      if (!contrasenia || !confirma_contrasenia) {
        return null;
      }

      // Solo validar si ambos campos tienen valor
      if (contrasenia.value && confirma_contrasenia.value) {
        if (contrasenia.value !== confirma_contrasenia.value) {
          return { passwordMismatch: true };
        }
      }

      return null;
    };
  }

  private initForm() {
    // Crear el FormGroup base
    const formGroup: any = {
      nombreCompleto: [this.usuario.nombreCompleto || '', [Validators.required, Validators.minLength(2)]],
      email: [this.usuario.email || '', [Validators.required, Validators.email]],
      usuarioNombre: [this.usuario.usuarioNombre || '', [Validators.required, Validators.minLength(3)]],
      rolId: [{ value: this.usuario.rolId || undefined, disabled: true }, Validators.required],
      activo: [this.usuario.activo ?? true]
    };

    // Si es creación, agregar solo campos de contraseña (sin checkbox)
    if (!this.usuario.id) {
      formGroup.contrasenia = ['', [Validators.required, Validators.minLength(6)]];
      formGroup.confirma_contrasenia = ['', [Validators.required, Validators.minLength(6)]];
    } else {
      // Si es edición, agregar solo el checkbox (sin campos de contraseña inicialmente)
      formGroup.permiteEditarContrasenia = [this.usuario.PermiteEditarContrasenia ?? false];

      // Los campos de contraseña se agregarán dinámicamente si se activa el checkbox
    }

    this.usuarioForm = this.fb.group(formGroup, {
      // Aplicar validador a nivel del formulario solo si hay campos de contraseña
      validators: this.hasPasswordFields() ? this.passwordMatchValidator() : null
    });

    // Si es edición, actualizar el formulario con los datos existentes
    if (this.usuario.id) {
      this.usuarioForm.patchValue(this.usuario);

      // Suscribirse a cambios del checkbox en modo edición
      this.usuarioForm.get('permiteEditarContrasenia')?.valueChanges.subscribe(permiteEditar => {
        this.togglePasswordFields(permiteEditar);
      });
    }
  }

  private cargarRoles() {
    this.loadingRoles.set(true);
    this.usuarioService.getRolesSelectList().subscribe({
      next: (roles) => {
        this.roles.set(roles.data);
        this.loadingRoles.set(false);
        // Habilitar el campo rolId cuando se carguen los roles
        this.usuarioForm.get('rolId')?.enable();
      },
      error: (error) => {
        console.error('Error al cargar roles:', error);
        this.utilsService.showNotification('Error', 'Error al cargar los roles', 'error');
        this.loadingRoles.set(false);
      }
    });
  }

  onSubmit() {
    if (this.usuarioForm.valid) {
      const formValue = this.usuarioForm.value;

      // Crear objeto usuario con los valores del formulario
      const usuarioData = {
        ...this.usuario,
        ...formValue
      };

      this.guardar.emit(usuarioData as CreateUsuarioDto | UpdateUsuarioDto);

      // Mostrar notificación según el contexto
      this.utilsService.showNotification('Éxito', 'Guardado Satisfactorio', 'success');

      if (!this.isFromProfile) {
        this.volverALista.emit();
      }
    } else {
      // Marcar todos los campos como touched para mostrar errores
      this.markFormGroupTouched();
    }
  }

  // Método para marcar todos los campos como touched
  private markFormGroupTouched() {
    Object.keys(this.usuarioForm.controls).forEach(key => {
      const control = this.usuarioForm.get(key);
      control?.markAsTouched();
    });
  }

  // Métodos helper para el template
  isFieldInvalid(fieldName: string): boolean {
    const field = this.usuarioForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.usuarioForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) return 'Este campo es requerido';
      if (field.errors['email']) return 'El email no es válido';
      if (field.errors['minlength']) return `Mínimo ${field.errors['minlength'].requiredLength} caracteres`;
      if (field.errors['passwordMismatch']) return 'Las contraseñas no coinciden';
    }

    // Verificar errores del formulario completo para contraseñas
    const formErrors = this.usuarioForm.errors;
    if (formErrors && formErrors['passwordMismatch']) {
      if (fieldName === 'contrasenia' || fieldName === 'confirma_contrasenia') {
        return 'Las contraseñas no coinciden';
      }
    }

    return '';
  }

  onCancel() {
    this.cancelar.emit();
  }

  // Método para verificar si el formulario tiene campos de contraseña
  private hasPasswordFields(): boolean {
    return this.usuarioForm?.contains('contrasenia') && this.usuarioForm?.contains('confirma_contrasenia');
  }

  // Método para mostrar/ocultar campos de contraseña en modo edición
  private togglePasswordFields(mostrar: boolean) {
    if (mostrar) {
      // Agregar campos de contraseña
      this.usuarioForm.addControl('contrasenia', this.fb.control('', [Validators.required, Validators.minLength(6)]));
      this.usuarioForm.addControl('confirma_contrasenia', this.fb.control('', [Validators.required, Validators.minLength(6)]));

      // Agregar validador de contraseñas
      this.usuarioForm.setValidators(this.passwordMatchValidator());
    } else {
      // Remover campos de contraseña
      this.usuarioForm.removeControl('contrasenia');
      this.usuarioForm.removeControl('confirma_contrasenia');

      // Remover validador de contraseñas
      this.usuarioForm.clearValidators();
    }

    // Actualizar validadores
    this.usuarioForm.updateValueAndValidity();
  }
}
