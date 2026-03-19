import { Component, OnInit, inject, signal, ViewChild, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { OperacionesService } from '../../../../api/services/operaciones.service';
import { SelectListsService } from '../../../../api/services/selectLists.service';
import { CargoAdicionalViewDto } from '../../../../api/models/cargoAdicionalViewDto';
import { CargoAdicionalDto } from '../../../../api/models/cargoAdicionalDto';
import { MovimientoItemDto } from '../../../../api/models/movimientoItemDto';
import { SelectItemDto } from '../../../../api/models/selectItemDto';
import { TipoMovimientoConfigDto } from '../../../../api/models/tipoMovimientoConfigDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';
import { SearchInputComponent } from '../../../shared/components/search-input/search-input.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '../../../shared/components/generic-table/table-column.model';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-cargo-adicional',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SearchInputComponent, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './cargo-adicional.component.html',
})
export class CargoAdicionalComponent implements OnInit {
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc      = inject(SelectListsService);
  private readonly utilsService   = inject(UtilsService);
  private readonly fb             = inject(FormBuilder);
  private readonly destroyRef     = inject(DestroyRef);

  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  // ── Catálogos ────────────────────────────────────────────────
  tiposMovimiento      = signal<SelectItemDto[]>([]);
  tipoMovimientoConfig = signal<TipoMovimientoConfigDto | null>(null);

  // ── Estado búsqueda ──────────────────────────────────────────
  contratoBusqueda = signal('');
  buscando         = signal(false);
  errorBusqueda    = signal<string | null>(null);

  // ── Datos ────────────────────────────────────────────────────
  info        = signal<CargoAdicionalViewDto | null>(null);
  movimientos = signal<MovimientoItemDto[]>([]);

  // ── Estado formulario ────────────────────────────────────────
  mostrarForm  = signal(false);
  guardando    = signal(false);
  editandoId   = signal<number | null>(null);   // null = nuevo
  eliminandoMov: MovimientoItemDto | null = null;

  // ── Columnas tabla ───────────────────────────────────────────
  readonly columnas: TableColumn[] = [
    { key: 'noPago',        header: 'No. Pago' },
    { key: 'fecMovimiento', header: 'Fecha',    type: 'date' },
    { key: 'descripcion',   header: 'Descripción' },
    { key: 'capital',       header: 'Capital',  type: 'currency' },
    { key: 'interes',       header: 'Interés',  type: 'currency' },
    { key: 'iva',           header: 'IVA',      type: 'currency' },
    { key: 'total',         header: 'Total',    type: 'currency' },
  ];

  readonly acciones: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-to-square', btnClass: 'btn-action-edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can',     btnClass: 'btn-action-delete' },
  ];

  // ── Formulario ───────────────────────────────────────────────
  form = this.fb.group({
    descripcion:      ['', Validators.required],
    fecMovimiento:    ['', Validators.required],
    idTipoMovimiento: [null as number | null, Validators.required],
    capital:          [null as number | null],
    interes:          [null as number | null],
    iva:              [null as number | null],
    total:            [null as number | null, [Validators.required, Validators.min(0.01)]],
  });

  constructor() {
    this.form.get('idTipoMovimiento')!.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(idTipo => {
        this.tipoMovimientoConfig.set(null);

        if (!idTipo) return;

        // Copiar descripción del tipo seleccionado
        const tipoSeleccionado = this.tiposMovimiento().find(t => t.value === idTipo);
        if (tipoSeleccionado?.text) {
          this.form.get('descripcion')!.setValue(tipoSeleccionado.text, { emitEvent: false });
        }

        // Cargar config: generaIVA_Capital / generaIVA_Interes
        this.operacionesSvc.getTipoMovimientoConfig(idTipo).subscribe({
          next: (res) => {
            if (res.success && res.data) {
              this.tipoMovimientoConfig.set(res.data);
            }
          },
          error: (err) => {
            if (!wasHandledByInterceptor(err)) {
              this.utilsService.showNotification('Error', 'Error al cargar configuración del movimiento', 'error');
            }
          },
        });
      });
  }

  ngOnInit(): void {
    this.selectSvc.getTipoMovimientos().subscribe({
      next: (res) => this.tiposMovimiento.set(res.data ?? []),
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar tipos de movimiento', 'error');
        }
      },
    });
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  // ── Búsqueda ─────────────────────────────────────────────────

  onSearch(value: string): void {
    this.contratoBusqueda.set(value);
    const contrato = value.trim();
    if (!contrato) return;

    this.buscando.set(true);
    this.errorBusqueda.set(null);
    this.info.set(null);
    this.movimientos.set([]);
    this.mostrarForm.set(false);
    this.editandoId.set(null);

    this.operacionesSvc.getCargoAdicional(contrato).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.info.set(res.data);
          this.movimientos.set(res.data.movimientos ?? []);
        } else {
          this.errorBusqueda.set(res.errors?.[0] ?? res.message ?? 'Contrato no encontrado');
        }
        this.buscando.set(false);
      },
      error: (err) => {
        this.buscando.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.errorBusqueda.set('Error de conexión al buscar el contrato');
        }
      },
    });
  }

  // ── Nuevo cargo ───────────────────────────────────────────────

  onNuevo(): void {
    const idContrato = this.info()?.idContratoPasivo;
    if (!idContrato) return;

    this.editandoId.set(null);
    this.form.reset();

    this.operacionesSvc.getNewCargoAdicional(idContrato).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.form.patchValue({
            fecMovimiento: res.data.fecMovimiento?.substring(0, 10) ?? '',
          });
          this.mostrarForm.set(true);
        } else {
          const msg = res.message ?? 'No es posible agregar un cargo adicional a este contrato';
          this.utilsService.showNotification('Aviso', msg, 'warning');
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al inicializar el cargo', 'error');
        }
      },
    });
  }

  onCancelarForm(): void {
    this.mostrarForm.set(false);
    this.editandoId.set(null);
    this.form.reset();
  }

  // ── Acción de tabla ───────────────────────────────────────────

  onAccion(event: TableActionEvent): void {
    const mov = event.row as MovimientoItemDto;
    if (event.action === 'edit') {
      this.editandoId.set(mov.idMovimiento ?? null);
      this.form.patchValue({
        descripcion:      mov.descripcion      ?? '',
        fecMovimiento:    mov.fecMovimiento?.substring(0, 10) ?? '',
        idTipoMovimiento: null,
        capital:          mov.capital          ?? null,
        interes:          mov.interes          ?? null,
        iva:              mov.iva              ?? null,
        total:            mov.total            ?? null,
      });
      this.mostrarForm.set(true);
    } else if (event.action === 'delete') {
      this.eliminandoMov = mov;
      this.confirmModal.show();
    }
  }

  // ── Guardar (create / update) ────────────────────────────────

  onGuardar(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const v = this.form.getRawValue();
    const idContrato = this.info()!.idContratoPasivo!;

    const cfg = this.tipoMovimientoConfig();
    const dto: CargoAdicionalDto = {
      idContrato,
      descripcion:       v.descripcion!,
      fecMovimiento:     v.fecMovimiento!,
      idTipoMovimiento:  v.idTipoMovimiento!,
      capital:           v.capital  ?? 0,
      interes:           v.interes  ?? 0,
      iva:               v.iva      ?? 0,
      total:             v.total!,
      generaIVA_Capital: cfg?.generaIVA_Capital ?? 0,
      generaIVA_Interes: cfg?.generaIVA_Interes ?? 0,
    };

    this.guardando.set(true);
    const idMov = this.editandoId();
    const op$ = idMov
      ? this.operacionesSvc.updateCargoAdicional(idMov, dto)
      : this.operacionesSvc.createCargoAdicional(dto);

    op$.subscribe({
      next: () => {
        this.guardando.set(false);
        const msg = idMov ? 'Cargo actualizado correctamente' : 'Cargo registrado correctamente';
        this.utilsService.showNotification('Éxito', msg, 'success');
        this.onCancelarForm();
        this.recargar();
      },
      error: (err) => {
        this.guardando.set(false);
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al guardar el cargo';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  // ── Eliminar ─────────────────────────────────────────────────

  confirmarEliminacion(): void {
    const idMov = this.eliminandoMov?.idMovimiento;
    if (!idMov) return;
    this.confirmModal.hide();

    this.operacionesSvc.deleteCargoAdicional(idMov).subscribe({
      next: () => {
        this.utilsService.showNotification('Éxito', 'Cargo eliminado correctamente', 'success');
        this.eliminandoMov = null;
        this.recargar();
      },
      error: (err) => {
        this.eliminandoMov = null;
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? 'Error al eliminar el cargo';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  // ── Private ──────────────────────────────────────────────────

  private recargar(): void {
    const contrato = this.contratoBusqueda().trim();
    if (!contrato) return;

    this.operacionesSvc.getCargoAdicional(contrato).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.info.set(res.data);
          this.movimientos.set(res.data.movimientos ?? []);
        }
      },
    });
  }
}
