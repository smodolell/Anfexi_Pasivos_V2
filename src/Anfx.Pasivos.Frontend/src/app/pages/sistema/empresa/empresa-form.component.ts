import { Component, OnInit, Input, Output, EventEmitter, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EmpresaDto, CreateEmpresaDto, UpdateEmpresaDto, TipoDireccionItemDto } from '../../../../types/sistema/empresa.dto';
import { UtilsService } from '../../../services/utils.service';
import { EmpresaService } from '../../../services/sistema/empresa.service';
import { ColoniaAutoCompleteComponent, ColoniaModel } from "../../../shared/components/colonia-auto-complete/colonia-auto-complete.component";

@Component({
  selector: 'app-empresa-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ColoniaAutoCompleteComponent],
  templateUrl: './empresa-form.component.html'
})
export class EmpresaFormComponent implements OnInit {
  @Input() empresa: Partial<EmpresaDto> = {
    tipoDireccionId: undefined,
  };
  @Output() guardar = new EventEmitter<CreateEmpresaDto | UpdateEmpresaDto>();
  @Output() cancelar = new EventEmitter<void>();
  @Output() volverALista = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private utilsService = inject(UtilsService);
  private empresaService = inject(EmpresaService);

  form: FormGroup;
  tiposDirecciones = signal<TipoDireccionItemDto[]>([]);
  loadingTiposDirecciones = signal<boolean>(false);

  constructor() {
    this.form = this.fb.group({
      sEmpresa: ['', [Validators.required, Validators.maxLength(100)]],
      rFC: ['', [Validators.required, Validators.maxLength(13)]],
      razonSocial: ['', [Validators.required, Validators.maxLength(200)]],
      telefono: ['', [Validators.maxLength(20)]],
      representante: ['', [Validators.maxLength(100)]],
      avisosEstadodeCuenta: ['', [Validators.maxLength(500)]],
      advertenciasEstadodeCuenta: ['', [Validators.maxLength(500)]],
      aclaracionesEstadodeCuenta: ['', [Validators.maxLength(500)]],
      usaDesembolso: [false],
      pasivo: [false],
      tipoDireccionId: [null, [Validators.required]],
      calle: ['', [Validators.maxLength(100)]],
      numExterior: ['', [Validators.maxLength(10)]],
      numInterior: ['', [Validators.maxLength(10)]],
      coloniaId: [null, [Validators.required]],
      direccion: [null] // Campo para el componente ColoniaAutoComplete
    });
  }

  ngOnInit(): void {
    this.cargarTiposDirecciones();
    this.setupColoniaSync();

    if (this.empresa) {
      this.form.patchValue({
        sEmpresa: this.empresa.sEmpresa,
        rFC: this.empresa.rFC,
        razonSocial: this.empresa.razonSocial,
        telefono: this.empresa.telefono,
        representante: this.empresa.representante,
        avisosEstadodeCuenta: this.empresa.avisosEstadodeCuenta,
        advertenciasEstadodeCuenta: this.empresa.advertenciasEstadodeCuenta,
        aclaracionesEstadodeCuenta: this.empresa.aclaracionesEstadodeCuenta,
        usaDesembolso: this.empresa.usaDesembolso,
        pasivo: this.empresa.pasivo,
        tipoDireccionId: this.empresa.tipoDireccionId,
        calle: this.empresa.calle,
        numExterior: this.empresa.numExterior,
        numInterior: this.empresa.numInterior,
        coloniaId: this.empresa.coloniaId
      });

      // Si hay coloniaId, configurar el campo direccion para el componente
      if (this.empresa.coloniaId) {
        this.form.patchValue({
          direccion: {
            codigoPostal: '',
            estado: '',
            municipio: '',
            coloniaId: this.empresa.coloniaId
          }
        });
      }
    }
  }

  private setupColoniaSync(): void {
    // Sincronizar cambios del componente ColoniaAutoComplete con el campo coloniaId
    this.form.get('direccion')?.valueChanges.subscribe((direccionData: ColoniaModel | null) => {
      if (direccionData && direccionData.coloniaId) {
        this.form.patchValue({ coloniaId: direccionData.coloniaId }, { emitEvent: false });
      }
    });
  }

  private cargarTiposDirecciones(): void {
    this.loadingTiposDirecciones.set(true);
    // Deshabilitar el control mientras se cargan los datos
    this.form.get('tipoDireccionId')?.disable();

    this.empresaService.getTiposDirecciones().subscribe({
      next: (response) => {
        if (response.success) {
          this.tiposDirecciones.set(response.data);
        } else {
          this.utilsService.showNotification('Error', 'Error al cargar tipos de direcciones', 'error');
        }
        this.loadingTiposDirecciones.set(false);
        // Habilitar el control cuando termine la carga
        this.form.get('tipoDireccionId')?.enable();
      },
      error: (httpError) => {
        this.utilsService.showNotification('Error', 'Error de conexión al cargar tipos de direcciones', 'error');
        console.error('Error HTTP:', httpError);
        this.loadingTiposDirecciones.set(false);
        // Habilitar el control incluso si hay error
        this.form.get('tipoDireccionId')?.enable();
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formData = this.form.value;

      if (this.empresa && this.empresa.id) {
        // Actualizar
        const updateData: UpdateEmpresaDto = {
          ...formData
        };
        this.guardar.emit(updateData);
      } else {
        // Crear
        const createData: CreateEmpresaDto = {
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
