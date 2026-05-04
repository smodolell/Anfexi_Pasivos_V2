import {
  Component, Input, Output, EventEmitter,
  OnInit, OnChanges, SimpleChanges, ViewChild, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TasaVariableDetalleDto } from '@api/models/tasaVariableDetalleDto';
import { TasaValorListItemDto } from '@api/models/tasaValorListItemDto';
import { CatalogosService } from '@api/services/catalogos.service';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';
import { CardComponent } from '@shared/components/card/card.component';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-tasa-variable-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CardComponent, ConfirmModalComponent],
  templateUrl: './tasa-variable-form.component.html'
})
export class TasaVariableFormComponent implements OnInit, OnChanges {
  @ViewChild('confirmValorModal') confirmValorModal!: ConfirmModalComponent;

  private fb                = inject(FormBuilder);
  private catalogosService  = inject(CatalogosService);
  private utilsService      = inject(UtilsService);

  @Input() tasa: TasaVariableDetalleDto | null = null;

  @Output() guardar        = new EventEmitter<string>();
  @Output() cancelar       = new EventEmitter<void>();
  @Output() valoresChanged = new EventEmitter<void>();

  form!: FormGroup;
  valorForm!: FormGroup;

  valores = signal<TasaValorListItemDto[]>([]);
  mostrandoValorForm = signal(false);
  valorAEditar: TasaValorListItemDto | null = null;
  valorAEliminar: TasaValorListItemDto | null = null;
  guardandoValor = signal(false);

  get isEditMode(): boolean { return !!this.tasa?.id; }

  ngOnInit() {
    this.initForms();
    if (this.tasa?.valores) {
      this.valores.set(this.tasa.valores);
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.form && changes['tasa']) {
      this.form.patchValue({ nombre: this.tasa?.nombre || '' });
      this.valores.set(this.tasa?.valores ?? []);
    }
  }

  private initForms() {
    this.form = this.fb.group({
      nombre: [this.tasa?.nombre || '', [Validators.required, Validators.maxLength(150)]]
    });
    this.valorForm = this.fb.group({
      valorTasa:      [null, [Validators.required, Validators.min(0)]],
      fecValorTasa:   ['', Validators.required],
      fecRegistroTasa: [null]
    });
  }

  onSubmit() {
    if (this.form.valid) {
      this.guardar.emit(this.form.value.nombre);
    } else {
      Object.values(this.form.controls).forEach(c => c.markAsTouched());
    }
  }

  isInvalid(field: string, formRef?: FormGroup): boolean {
    const f = formRef ?? this.form;
    const c = f.get(field);
    return !!(c && c.invalid && (c.dirty || c.touched));
  }

  getError(field: string, formRef?: FormGroup): string {
    const f = formRef ?? this.form;
    const c = f.get(field);
    if (c?.errors) {
      if (c.errors['required'])  return 'Este campo es requerido';
      if (c.errors['maxlength']) return `Máximo ${c.errors['maxlength'].requiredLength} caracteres`;
      if (c.errors['min'])       return `El valor mínimo es ${c.errors['min'].min}`;
    }
    return '';
  }

  // ── Gestión de valores ─────────────────────────────────────────

  onAgregarValor() {
    this.valorAEditar = null;
    this.valorForm.reset();
    this.mostrandoValorForm.set(true);
  }

  onEditarValor(valor: TasaValorListItemDto) {
    this.valorAEditar = valor;
    this.valorForm.patchValue({
      valorTasa:       valor.valorTasa,
      fecValorTasa:    valor.fecValorTasa    ? valor.fecValorTasa.substring(0, 10)    : '',
      fecRegistroTasa: valor.fecRegistroTasa ? valor.fecRegistroTasa.substring(0, 10) : null
    });
    this.mostrandoValorForm.set(true);
  }

  onEliminarValor(valor: TasaValorListItemDto) {
    this.valorAEliminar = valor;
    this.confirmValorModal.show();
  }

  confirmarEliminarValor() {
    if (!this.valorAEliminar) return;
    this.confirmValorModal.confirmLoading.set(true);
    this.catalogosService.deleteTasaValor(this.valorAEliminar.id!).subscribe({
      next: () => {
        this.confirmValorModal.confirmLoading.set(false);
        this.confirmValorModal.hide();
        this.utilsService.showNotification('Éxito', 'Valor eliminado correctamente', 'success');
        this.valores.update(v => v.filter(x => x.id !== this.valorAEliminar!.id));
        this.valorAEliminar = null;
        this.valoresChanged.emit();
      },
      error: (err) => {
        this.confirmValorModal.confirmLoading.set(false);
        this.confirmValorModal.hide();
        this.valorAEliminar = null;
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al eliminar el valor', 'error');
      }
    });
  }

  cancelarEliminarValor() {
    this.valorAEliminar = null;
  }

  onGuardarValor() {
    if (this.valorForm.invalid) {
      Object.values(this.valorForm.controls).forEach(c => c.markAsTouched());
      return;
    }

    this.guardandoValor.set(true);
    const dto = { ...this.valorForm.value };

    if (this.valorAEditar?.id) {
      this.catalogosService.updateTasaValor(this.valorAEditar.id, dto).subscribe({
        next: () => {
          this.utilsService.showNotification('Éxito', 'Valor actualizado correctamente', 'success');
          this.recargarValores();
        },
        error: (err) => {
          this.guardandoValor.set(false);
          if (!wasHandledByInterceptor(err))
            this.utilsService.showNotification('Error', 'Error al actualizar el valor', 'error');
        }
      });
    } else {
      this.catalogosService.createTasaValor(this.tasa!.id!, dto).subscribe({
        next: () => {
          this.utilsService.showNotification('Éxito', 'Valor agregado correctamente', 'success');
          this.recargarValores();
        },
        error: (err) => {
          this.guardandoValor.set(false);
          if (!wasHandledByInterceptor(err))
            this.utilsService.showNotification('Error', 'Error al agregar el valor', 'error');
        }
      });
    }
  }

  onCancelarValor() {
    this.mostrandoValorForm.set(false);
    this.valorAEditar = null;
    this.valorForm.reset();
  }

  private recargarValores() {
    this.catalogosService.getTasaVariableById(this.tasa!.id!).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.valores.set(res.data.valores ?? []);
          this.valoresChanged.emit();
        }
        this.guardandoValor.set(false);
        this.mostrandoValorForm.set(false);
        this.valorAEditar = null;
        this.valorForm.reset();
      },
      error: () => { this.guardandoValor.set(false); }
    });
  }
}
