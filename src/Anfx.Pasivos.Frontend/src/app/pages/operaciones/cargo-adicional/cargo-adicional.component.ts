import { Component, computed, inject, signal, ViewChild, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  EMPTY, Subject,
  exhaustMap, switchMap, map, timeout, catchError, tap, filter,
} from 'rxjs';
import { OperacionesService } from 'src/app/core/api/services/operaciones.service';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { AutocompleteResultDto } from 'src/app/core/api/models/autocompleteResultDto';
import { CargoAdicionalViewDto } from 'src/app/core/api/models/cargoAdicionalViewDto';
import { CargoAdicionalDto } from 'src/app/core/api/models/cargoAdicionalDto';
import { MovimientoItemDto } from 'src/app/core/api/models/movimientoItemDto';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { TipoMovimientoConfigDto } from 'src/app/core/api/models/tipoMovimientoConfigDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { ErrorHandlerService } from '@services/error.services';
import { ContratoAutocompleteComponent } from '@shared/components/contrato-autocomplete/contrato-autocomplete.component';
import { FormErrorsComponent } from '@shared/components/form-errors/form-error.component';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import {
  TableColumn,
  TableAction,
  TableActionEvent,
} from '@shared/components/generic-table/table-column.model';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';
import { LayoutService } from '@services/layout.service';
import { CardInfoComponent } from '@shared/components/card/card-info.component';

@Component({
  selector: 'app-cargo-adicional',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ContratoAutocompleteComponent,
    FormErrorsComponent,
    GenericTableComponent,
    ConfirmModalComponent,
    CardInfoComponent,
  ],
  templateUrl: './cargo-adicional.component.html',
})
export class CargoAdicionalComponent {
  private readonly layoutService = inject(LayoutService);
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly errorSvc = inject(ErrorHandlerService);

  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  tiposMovimiento = signal<SelectItemDto[]>([]);
  tipoMovimientoConfig = signal<TipoMovimientoConfigDto | null>(null);

  contratoBusqueda = signal('');
  buscando = signal(false);
  erroresBusqueda = signal<string[]>([]);

  info = signal<CargoAdicionalViewDto | null>(null);
  movimientos = signal<MovimientoItemDto[]>([]);

  mostrarForm = signal(false);
  guardando = signal(false);
  editandoId = signal<number | null>(null);
  eliminandoMov: MovimientoItemDto | null = null;

  private readonly _capital = signal<number>(0);
  private readonly _interes = signal<number>(0);

  readonly ivaCapital = computed(() =>
    this.round2(
      this._capital() *
        (this.info()?.tasaIva ?? 0) *
        (this.tipoMovimientoConfig()?.generaIVA_Capital ?? 0),
    ),
  );
  readonly ivaInteres = computed(() =>
    this.round2(
      this._interes() *
        (this.info()?.tasaIva ?? 0) *
        (this.tipoMovimientoConfig()?.generaIVA_Interes ?? 0),
    ),
  );
  readonly iva = computed(() => this.ivaCapital() + this.ivaInteres());
  readonly total = computed(() => this._capital() + this._interes() + this.iva());

  readonly columnas: TableColumn[] = [
    { key: 'noPago', header: 'No. Pago' },
    { key: 'fecMovimiento', header: 'Fecha', type: 'date' },
    { key: 'descripcion', header: 'Descripción' },
    { key: 'capital', header: 'Capital', type: 'currency' },
    { key: 'interes', header: 'Interés', type: 'currency' },
    { key: 'iva', header: 'IVA', type: 'currency' },
    { key: 'total', header: 'Total', type: 'currency' },
  ];

  readonly acciones: TableAction[] = [
    { id: 'edit', label: 'Editar', icon: 'fa-solid fa-pen-to-square', btnClass: 'btn-action-edit' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', btnClass: 'btn-action-delete' },
  ];

  form = this.fb.group({
    descripcion: ['', Validators.required],
    fecMovimiento: ['', Validators.required],
    idTipoMovimiento: [null as number | null, Validators.required],
    capital: [null as number | null],
    interes: [null as number | null],
  });

  private readonly buscar$ = new Subject<string>();
  private readonly guardar$ = new Subject<void>();
  private readonly eliminar$ = new Subject<number>();
  private readonly nuevo$ = new Subject<number>();
  private readonly recargar$ = new Subject<void>();

  constructor() {
    this.layoutService.setTitle('Cargo Adicional Contrato');
    this.wireCatalogos();
    this.wireCapitalInteresChanges();
    this.wireTipoMovimiento();
    this.wireBuscar();
    this.wireNuevo();
    this.wireGuardar();
    this.wireEliminar();
    this.wireRecargar();
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onContratoSelected(item: AutocompleteResultDto): void {
    const contrato = item.label?.trim();
    if (!contrato) return;
    this.contratoBusqueda.set(contrato);
    this.buscar$.next(contrato);
  }

  onNuevo(): void {
    const idContrato = this.info()?.idContratoPasivo;
    if (!idContrato) return;
    this.editandoId.set(null);
    this.form.reset();
    this.nuevo$.next(idContrato);
  }

  onCancelarForm(): void {
    this.mostrarForm.set(false);
    this.editandoId.set(null);
    this.tipoMovimientoConfig.set(null);
    this._capital.set(0);
    this._interes.set(0);
    this.form.reset();
  }

  onAccion(event: TableActionEvent): void {
    const mov = event.row as MovimientoItemDto;
    if (event.action === 'edit') {
      this.editandoId.set(mov.idMovimiento ?? null);
      this.form.patchValue(
        {
          descripcion: mov.descripcion ?? '',
          fecMovimiento: mov.fecMovimiento?.substring(0, 10) ?? '',
          capital: mov.capital ?? null,
          interes: mov.interes ?? null,
        },
        { emitEvent: false },
      );
      this._capital.set(mov.capital ?? 0);
      this._interes.set(mov.interes ?? 0);
      this.form.get('idTipoMovimiento')!.setValue(mov.idTipoMovimiento ?? null);
      this.mostrarForm.set(true);
    } else if (event.action === 'delete') {
      this.eliminandoMov = mov;
      this.confirmModal.show();
    }
  }

  onGuardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.guardar$.next();
  }

  confirmarEliminacion(): void {
    const idMov = this.eliminandoMov?.idMovimiento;
    if (!idMov) return;
    this.confirmModal.hide();
    this.eliminar$.next(idMov);
  }

  private wireCatalogos(): void {
    this.selectSvc.getTipoMovimientosCapturables().pipe(
      timeout(30_000),
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar tipos de movimiento', 'error');
        }
        return EMPTY;
      }),
    ).subscribe((res) => this.tiposMovimiento.set(res.data ?? []));
  }

  private wireCapitalInteresChanges(): void {
    this.form.get('capital')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((v) => this._capital.set(v ?? 0));

    this.form.get('interes')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((v) => this._interes.set(v ?? 0));
  }

  private wireTipoMovimiento(): void {
    this.form.get('idTipoMovimiento')!.valueChanges.pipe(
      tap((idTipo) => {
        this.tipoMovimientoConfig.set(null);
        if (idTipo) {
          const tipo = this.tiposMovimiento().find((t) => t.value === idTipo);
          if (tipo?.text) {
            this.form.get('descripcion')!.setValue(tipo.text, { emitEvent: false });
          }
        }
      }),
      filter((idTipo): idTipo is number => !!idTipo),
      switchMap((idTipo) =>
        this.operacionesSvc.getTipoMovimientoConfig(idTipo).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            if (!wasHandledByInterceptor(err)) {
              this.utilsService.showNotification(
                'Error',
                'Error al cargar configuración del movimiento',
                'error',
              );
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      if (res.success && res.data) {
        this.tipoMovimientoConfig.set(res.data);
      }
    });
  }

  private wireBuscar(): void {
    this.buscar$.pipe(
      tap(() => {
        this.buscando.set(true);
        this.erroresBusqueda.set([]);
        this.info.set(null);
        this.movimientos.set([]);
        this.mostrarForm.set(false);
        this.editandoId.set(null);
      }),
      switchMap((contrato) =>
        this.operacionesSvc.getCargoAdicional(contrato).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.buscando.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.erroresBusqueda.set(this.errorSvc.parseError(err));
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      if (res.success && res.data) {
        this.info.set(res.data);
        this.movimientos.set(res.data.movimientos ?? []);
      } else {
        this.erroresBusqueda.set([res.errors?.[0] ?? res.message ?? 'Contrato no encontrado']);
      }
      this.buscando.set(false);
    });
  }

  private wireNuevo(): void {
    this.nuevo$.pipe(
      switchMap((idContrato) =>
        this.operacionesSvc.getNewCargoAdicional(idContrato).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            if (!wasHandledByInterceptor(err)) {
              this.utilsService.showNotification('Error', 'Error al inicializar el cargo', 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      if (res.success && res.data) {
        this.form.patchValue({
          fecMovimiento: res.data.fecMovimiento?.substring(0, 10) ?? '',
        });
        this.mostrarForm.set(true);
      } else {
        const msg = res.message ?? 'No es posible agregar un cargo adicional a este contrato';
        this.utilsService.showNotification('Aviso', msg, 'warning');
      }
    });
  }

  private wireGuardar(): void {
    this.guardar$.pipe(
      exhaustMap(() => {
        const idMov = this.editandoId();
        const v = this.form.getRawValue();
        const idContrato = this.info()!.idContratoPasivo!;
        const cfg = this.tipoMovimientoConfig();

        const dto: CargoAdicionalDto = {
          idContrato,
          descripcion: v.descripcion!,
          fecMovimiento: v.fecMovimiento!,
          idTipoMovimiento: v.idTipoMovimiento!,
          capital: v.capital ?? 0,
          interes: v.interes ?? 0,
          iva: this.iva(),
          total: this.total(),
          porcIVA: this.info()?.tasaIva ?? 0,
          generaIVA_Capital: cfg?.generaIVA_Capital ?? 0,
          generaIVA_Interes: cfg?.generaIVA_Interes ?? 0,
        };

        this.guardando.set(true);
        const op$ = idMov
          ? this.operacionesSvc.updateCargoAdicional(idMov, dto)
          : this.operacionesSvc.createCargoAdicional(dto);

        return op$.pipe(
          timeout(30_000),
          map(() => idMov),
          catchError((err: unknown) => {
            this.guardando.set(false);
            if (!wasHandledByInterceptor(err)) {
              const e = err as { error?: { message?: string; errors?: string[] } };
              const msg = e.error?.message ?? e.error?.errors?.[0] ?? 'Error al guardar el cargo';
              this.utilsService.showNotification('Error', msg, 'error');
            }
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((idMov) => {
      this.guardando.set(false);
      const msg = idMov ? 'Cargo actualizado correctamente' : 'Cargo registrado correctamente';
      this.utilsService.showNotification('Éxito', msg, 'success');
      this.onCancelarForm();
      this.recargar$.next();
    });
  }

  private wireEliminar(): void {
    this.eliminar$.pipe(
      exhaustMap((idMov) =>
        this.operacionesSvc.deleteCargoAdicional(idMov).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.eliminandoMov = null;
            if (!wasHandledByInterceptor(err)) {
              const e = err as { error?: { message?: string } };
              const msg = e.error?.message ?? 'Error al eliminar el cargo';
              this.utilsService.showNotification('Error', msg, 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(() => {
      this.utilsService.showNotification('Éxito', 'Cargo eliminado correctamente', 'success');
      this.eliminandoMov = null;
      this.recargar$.next();
    });
  }

  private wireRecargar(): void {
    this.recargar$.pipe(
      switchMap(() => {
        const contrato = this.contratoBusqueda().trim();
        if (!contrato) return EMPTY;
        return this.operacionesSvc.getCargoAdicional(contrato).pipe(
          timeout(30_000),
          catchError(() => EMPTY),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      if (res.success && res.data) {
        this.info.set(res.data);
        this.movimientos.set(res.data.movimientos ?? []);
      }
    });
  }

  private round2(value: number): number {
    return Math.round(value * 100) / 100;
  }
}
