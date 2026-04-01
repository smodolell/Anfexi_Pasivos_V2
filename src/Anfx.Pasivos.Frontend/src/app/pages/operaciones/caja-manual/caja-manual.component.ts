import { Component, inject, signal, computed, ViewChild, ElementRef, effect } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { forkJoin } from 'rxjs';
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
import { ErrorHandlerService }  from '@services/error.services';
import { CardInfoComponent } from '@shared/components/card/card-info.component';

@Component({
  selector: 'app-caja-manual',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, DecimalPipe, ConfirmModalComponent, ContratoAutocompleteComponent, FormErrorsComponent, CardInfoComponent],
  templateUrl: './caja-manual.component.html',
})
export class CajaManualComponent {
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb = inject(FormBuilder);
  private readonly errorSvc = inject(ErrorHandlerService);

  @ViewChild('selectAllCb') selectAllCb!: ElementRef<HTMLInputElement>;
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  // ── Catálogos ────────────────────────────────────────────────
  tiposPago = signal<SelectItemDto[]>([]);
  bancos = signal<SelectItemDto[]>([]);
  cuentasBancarias = signal<SelectItemDto[]>([]);
  cargandoCuentas = signal(false);

  // ── Estado búsqueda ──────────────────────────────────────────
  contratoBusqueda = signal<string>('');
  buscando = signal(false);
  erroresBusqueda = signal<string[]>([]);

  // ── Datos cargados ───────────────────────────────────────────
  datosCaja = signal<CajaDto | null>(null);
  movimientos = signal<MovimientoPagoItem[]>([]);
  guardando = signal(false);
  erroresConfirmacion = signal<string[]>([]);

  // ── Computed ─────────────────────────────────────────────────
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

  // ── Formulario ───────────────────────────────────────────────
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

    // Cuando cambia el banco → cargar cuentas bancarias de ese banco
    this.form.get('idBanco')!.valueChanges.subscribe((idBanco) => {
      this.cuentasBancarias.set([]);
      this.form.get('idCuentaBancaria')!.setValue(null, { emitEvent: false });

      if (idBanco) {
        this.cargandoCuentas.set(true);
        this.selectSvc.getCuentaBancariaByBancoIdSelectList(idBanco).subscribe({
          next: (res) => {
            this.cuentasBancarias.set(res.data ?? []);
            this.cargandoCuentas.set(false);
          },
          error: (err: unknown) => {
            this.cargandoCuentas.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.utilsService.showNotification(
                'Error',
                'Error al cargar cuentas bancarias',
                'error',
              );
            }
          },
        });
      }
    });
  }

  get mostrarFormulario(): boolean {
    return this.datosCaja() !== null;
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  // ── Búsqueda ─────────────────────────────────────────────────

  onContratoSelected(item: AutocompleteResultDto): void {
    const contrato = item.label?.trim();
    if (!contrato) return;

    this.contratoBusqueda.set(contrato);
    this.buscando.set(true);
    this.erroresBusqueda.set([]);
    this.datosCaja.set(null);
    this.movimientos.set([]);
    this.form.reset();
    this.cuentasBancarias.set([]);

    const catalogosYaCargados = this.tiposPago().length > 0;

    if (catalogosYaCargados) {
      this.operacionesSvc.getCajaByContrato(contrato).subscribe({
        next: (res) => this.handleCajaResponse(res),
        error: (err) => this.handleBuscarError(err),
      });
    } else {
      forkJoin({
        caja: this.operacionesSvc.getCajaByContrato(contrato),
        tipoPago: this.selectSvc.getTipoPagos(),
        bancos: this.selectSvc.getBancosSelectList(),
      }).subscribe({
        next: ({ caja, tipoPago, bancos }) => {
          this.tiposPago.set(tipoPago.data ?? []);
          this.bancos.set(bancos.data ?? []);
          this.handleCajaResponse(caja);
        },
        error: (err) => this.handleBuscarError(err),
      });
    }
  }

  // ── Selección de movimientos ─────────────────────────────────

  toggleSelectAll(checked: boolean): void {
    this.movimientos.update((items) => items.map((m) => ({ ...m, seleccionado: checked })));
  }

  toggleMovimiento(index: number, checked: boolean): void {
    this.movimientos.update((items) =>
      items.map((m, i) => (i === index ? { ...m, seleccionado: checked } : m)),
    );
  }

  // ── Confirmar (abre modal) ────────────────────────────────────

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

  // ── Ejecutar pago (tras confirmación) ────────────────────────

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
    this.operacionesSvc.confirmarPagoCaja(dto).subscribe({
      next: (res) => {
        this.guardando.set(false);
        const msg = res?.message ?? 'Pago de caja procesado correctamente';
        this.utilsService.showNotification('Éxito', msg, 'success');
        this.recargarContrato();
      },
      error: (err: unknown) => {
        this.guardando.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.erroresConfirmacion.set(this.errorSvc.parseError(err));
        }
      },
    });
  }

  // ── Private ──────────────────────────────────────────────────

  private recargarContrato(): void {
    const contrato = this.contratoBusqueda().trim();
    if (!contrato) return;

    this.movimientos.set([]);
    this.cuentasBancarias.set([]);
    this.form.reset();

    this.operacionesSvc.getCajaByContrato(contrato).subscribe({
      next: (res) => this.handleCajaResponse(res),
      error: (err: unknown) => {
        if (!wasHandledByInterceptor(err)) {
           this.erroresBusqueda.set(this.errorSvc.parseError(err));
          this.utilsService.showNotification('Error', 'Error al recargar el contrato', 'error');
        }
      },
    });
  }

  private handleCajaResponse(res: { data?: CajaDto }): void {
    this.buscando.set(false);
    this.datosCaja.set(res.data!);
    this.movimientos.set(res.data!.movimientos ?? []);
    this.poblarFormulario(res.data!);
  }

  private handleBuscarError(err: unknown): void {
    this.buscando.set(false);
    if (!wasHandledByInterceptor(err)) {
      this.erroresBusqueda.set(this.errorSvc.parseError(err));
    }
  }

  private poblarFormulario(d: CajaDto): void {
    this.form.patchValue({
      idTipoPago: d.idTipoPago || null,
      idBanco: d.idBanco || null,
      fechaPago: d.fechaPago?.substring(0, 10) ?? '',
      referencia: d.referencia ?? null,
    });
    // idCuentaBancaria se setea después de que se carguen las cuentas del banco
    if (d.idBanco && d.idCuentaBancaria) {
      this.selectSvc.getCuentaBancariaByBancoIdSelectList(d.idBanco).subscribe({
        next: (res) => {
          this.cuentasBancarias.set(res.data ?? []);
          this.form
            .get('idCuentaBancaria')!
            .setValue(d.idCuentaBancaria ?? null, { emitEvent: false });
        },
      });
    }
  }
}
