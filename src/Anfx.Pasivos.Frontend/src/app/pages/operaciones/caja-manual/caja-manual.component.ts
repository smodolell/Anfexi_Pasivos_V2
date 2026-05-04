import { Component, inject, signal, computed, ViewChild, ElementRef, effect, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Subject, EMPTY, forkJoin, of, exhaustMap, switchMap, map, timeout, catchError, tap, filter } from 'rxjs';
import { OperacionesService } from 'src/app/core/api/services/operaciones.service';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { AutocompleteResultDto } from 'src/app/core/api/models/autocompleteResultDto';
import { CajaDto } from 'src/app/core/api/models/cajaDto';
import { MovimientoPagoItem } from 'src/app/core/api/models/movimientoPagoItem';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { ContratoAutocompleteComponent } from '@shared/components/contrato-autocomplete/contrato-autocomplete.component';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';
import { FormErrorsComponent } from '@shared/components/form-errors/form-error.component';
import { ErrorHandlerService } from '@services/error.services';
import { CardInfoComponent } from '@shared/components/card/card-info.component';

interface CargaResult {
  caja: CajaDto;
  cuentas: SelectItemDto[];
}

@Component({
  selector: 'app-caja-manual',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CurrencyPipe, DecimalPipe, ConfirmModalComponent, ContratoAutocompleteComponent, FormErrorsComponent, CardInfoComponent],
  templateUrl: './caja-manual.component.html',
})
export class CajaManualComponent {
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb = inject(FormBuilder);
  private readonly errorSvc = inject(ErrorHandlerService);
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild('selectAllCb') selectAllCb!: ElementRef<HTMLInputElement>;
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  tiposPago = signal<SelectItemDto[]>([]);
  bancos = signal<SelectItemDto[]>([]);
  cuentasBancarias = signal<SelectItemDto[]>([]);
  cargandoCuentas = signal(false);

  contratoBusqueda = signal<string>('');
  buscando = signal(false);
  erroresBusqueda = signal<string[]>([]);

  datosCaja = signal<CajaDto | null>(null);
  movimientos = signal<MovimientoPagoItem[]>([]);
  guardando = signal(false);
  erroresConfirmacion = signal<string[]>([]);

  private readonly buscar$ = new Subject<string>();
  private readonly pago$ = new Subject<CajaDto>();

  totalSeleccionados = computed(() => this.movimientos().filter((m) => m.seleccionado).length);

  allSelected = computed(
    () => this.movimientos().length > 0 && this.movimientos().every((m) => m.seleccionado),
  );

  someSelected = computed(
    () => this.movimientos().some((m) => m.seleccionado) && !this.allSelected(),
  );

  montoPago = computed(() =>
    this.movimientos()
      .filter((m) => m.seleccionado)
      .reduce((sum, m) => sum + (m.saldoTotal ?? 0), 0),
  );

  form = this.fb.group({
    idTipoPago: [null as number | null, Validators.required],
    idBanco: [null as number | null, Validators.required],
    idCuentaBancaria: [null as number | null, Validators.required],
    fechaPago: ['', Validators.required],
    referencia: [null as string | null],
  });

  constructor() {
    effect(() => {
      if (this.selectAllCb?.nativeElement) {
        this.selectAllCb.nativeElement.indeterminate = this.someSelected();
      }
    });

    this.wireBancoValueChanges();
    this.wireBuscar();
    this.wirePago();
  }

  get mostrarFormulario(): boolean {
    return this.datosCaja() !== null;
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

  toggleSelectAll(checked: boolean): void {
    this.movimientos.update((items) => items.map((m) => ({ ...m, seleccionado: checked })));
  }

  toggleMovimiento(index: number, checked: boolean): void {
    this.movimientos.update((items) =>
      items.map((m, i) => (i === index ? { ...m, seleccionado: checked } : m)),
    );
  }

  onConfirmar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (this.totalSeleccionados() === 0) {
      this.utilsService.showNotification('Aviso', 'Seleccione al menos un movimiento', 'warning');
      return;
    }
    this.confirmModal.show();
  }

  ejecutarPago(): void {
    this.confirmModal.hide();

    const seleccionados = this.movimientos().filter((m) => m.seleccionado);
    const v = this.form.getRawValue();
    const base = this.datosCaja()!;

    const dto: CajaDto = {
      idContrato: base.idContrato,
      idFondeador: base.idFondeador,
      idUsuario: base.idUsuario,
      contratoPasivo: base.contratoPasivo,
      fondeador: base.fondeador,
      idTipoPago: v.idTipoPago!,
      idBanco: v.idBanco!,
      idCuentaBancaria: v.idCuentaBancaria!,
      fechaPago: v.fechaPago!,
      referencia: v.referencia ?? null,
      montoPago: this.montoPago(),
      movimientos: seleccionados,
    };

    this.guardando.set(true);
    this.erroresConfirmacion.set([]);
    this.pago$.next(dto);
  }

  private wireBancoValueChanges(): void {
    this.form.get('idBanco')!.valueChanges.pipe(
      tap(() => {
        this.cuentasBancarias.set([]);
        this.form.get('idCuentaBancaria')!.setValue(null, { emitEvent: false });
      }),
      filter((idBanco): idBanco is number => !!idBanco),
      tap(() => this.cargandoCuentas.set(true)),
      switchMap((idBanco) =>
        this.selectSvc.getCuentaBancariaByBancoIdSelectList(idBanco).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.cargandoCuentas.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.utilsService.showNotification('Error', 'Error al cargar cuentas bancarias', 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.cuentasBancarias.set(res.data ?? []);
      this.cargandoCuentas.set(false);
    });
  }

  private wireBuscar(): void {
    this.buscar$.pipe(
      tap(() => {
        this.buscando.set(true);
        this.erroresBusqueda.set([]);
        this.datosCaja.set(null);
        this.movimientos.set([]);
        this.form.reset();
        this.cuentasBancarias.set([]);
      }),
      switchMap((contrato) => this.buildCargaStream(contrato)),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe({
      next: ({ caja, cuentas }) => {
        this.buscando.set(false);
        this.datosCaja.set(caja);
        this.movimientos.set(caja.movimientos ?? []);
        this.form.patchValue(
          {
            idTipoPago: caja.idTipoPago || null,
            idBanco: caja.idBanco || null,
            fechaPago: caja.fechaPago?.substring(0, 10) ?? '',
            referencia: caja.referencia ?? null,
          },
          { emitEvent: false },
        );
        if (cuentas.length > 0) {
          this.cuentasBancarias.set(cuentas);
          this.form.get('idCuentaBancaria')!.setValue(caja.idCuentaBancaria ?? null, { emitEvent: false });
        }
      },
      error: (err: unknown) => {
        this.buscando.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.erroresBusqueda.set(this.errorSvc.parseError(err));
        }
      },
    });
  }

  private wirePago(): void {
    this.pago$.pipe(
      exhaustMap((dto) =>
        this.operacionesSvc.confirmarPagoCaja(dto).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.guardando.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.erroresConfirmacion.set(this.errorSvc.parseError(err));
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.guardando.set(false);
      const msg = res?.message ?? 'Pago de caja procesado correctamente';
      this.utilsService.showNotification('Éxito', msg, 'success');
      this.buscar$.next(this.contratoBusqueda());
    });
  }

  private buildCargaStream(contrato: string) {
    const caja$ = this.operacionesSvc.getCajaByContrato(contrato).pipe(timeout(30_000));

    const base$ = this.tiposPago().length > 0
      ? caja$.pipe(map((res) => res.data!))
      : forkJoin({
          caja: caja$,
          tipoPago: this.selectSvc.getTipoPagos().pipe(timeout(30_000)),
          bancos: this.selectSvc.getBancosSelectList().pipe(timeout(30_000)),
        }).pipe(
          tap(({ tipoPago, bancos }) => {
            this.tiposPago.set(tipoPago.data ?? []);
            this.bancos.set(bancos.data ?? []);
          }),
          map(({ caja }) => caja.data!),
        );

    return base$.pipe(
      switchMap((caja) => {
        if (caja.idBanco && caja.idCuentaBancaria) {
          return this.selectSvc.getCuentaBancariaByBancoIdSelectList(caja.idBanco).pipe(
            timeout(30_000),
            map((res): CargaResult => ({ caja, cuentas: res.data ?? [] })),
            catchError(() => of<CargaResult>({ caja, cuentas: [] })),
          );
        }
        return of<CargaResult>({ caja, cuentas: [] });
      }),
    );
  }
}
