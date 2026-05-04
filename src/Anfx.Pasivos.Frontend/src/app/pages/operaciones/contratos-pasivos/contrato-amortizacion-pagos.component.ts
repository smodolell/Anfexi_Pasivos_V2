import {
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
  ElementRef,
  inject,
  signal,
  computed,
  ChangeDetectionStrategy,
  DestroyRef,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, EMPTY, switchMap, timeout, catchError } from 'rxjs';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { InfoGeneralContratoPasivoDto } from 'src/app/core/api/models/infoGeneralContratoPasivoDto';
import { PagoItemDto } from 'src/app/core/api/models/pagoItemDto';
import { MovimientoItemDto } from 'src/app/core/api/models/movimientoItemDto';
import { TablaAmortizaItemDto } from 'src/app/core/api/models/tablaAmortizaItemDto';
import { DetallePagoMovimientoDto } from 'src/app/core/api/models/detallePagoMovimientoDto';
import { DetalleMovimientoPagoDto } from 'src/app/core/api/models/detalleMovimientoPagoDto';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '@shared/components/generic-table/table-column.model';

export type TabActivaAmortizacion = 'tabla' | 'movimientos' | 'pagos';

interface BsModal {
  show(): void;
  hide(): void;
  dispose(): void;
}

declare const bootstrap: { Modal: new (el: HTMLElement) => BsModal };

@Component({
  selector: 'app-contrato-amortizacion-pagos',
  standalone: true,
  imports: [CommonModule, FormsModule, GenericTableComponent],
  templateUrl: './contrato-amortizacion-pagos.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContratoAmortizacionPagosComponent implements OnChanges {
  @Input() contrato: InfoGeneralContratoPasivoDto | null = null;

  @ViewChild('detalleModalEl') detalleModalEl!: ElementRef<HTMLElement>;
  @ViewChild('detallePagoModalEl') detallePagoModalEl!: ElementRef<HTMLElement>;

  private readonly service = inject(ContratosService);
  private readonly destroyRef = inject(DestroyRef);

  private bsModalDetalle?: BsModal;
  private bsModalDetallePago?: BsModal;

  pagos = signal<PagoItemDto[]>([]);
  movimientos = signal<MovimientoItemDto[]>([]);
  tablaAmortiza = signal<TablaAmortizaItemDto[]>([]);

  isLoadingTabla = signal(false);
  isLoadingDetalle = signal(false);
  isLoadingDetallePago = signal(false);

  detalleMovimiento = signal<DetallePagoMovimientoDto[]>([]);
  detallePago = signal<DetalleMovimientoPagoDto[]>([]);

  activeTab = signal<TabActivaAmortizacion>('tabla');
  tipoTabla = signal<number>(1);
  tiposTabla = [
    { value: 1, label: 'Actual' },
    { value: 2, label: 'Detallada' },
  ];

  private idContrato = 0;

  private readonly loadTabla$ = new Subject<number>();
  private readonly loadDetalleMovimiento$ = new Subject<number>();
  private readonly loadDetallePago$ = new Subject<number>();

  readonly columnasPagos: TableColumn[] = [
    { key: 'idPago', header: 'ID Pago' },
    { key: 'tipoPago', header: 'Tipo Pago' },
    { key: 'cuentaBancaria', header: 'Cuenta Bancaria' },
    { key: 'fecPagoValor', header: 'Fec. Valor', type: 'date' },
    { key: 'fecPagoRegistro', header: 'Fec. Registro', type: 'date' },
    { key: 'montoPago', header: 'Monto Pago', type: 'currency' },
    { key: 'montoAplicado', header: 'Monto Aplicado', type: 'currency' },
    { key: 'montoAplicadoOtros', header: 'Otros', type: 'currency' },
    { key: 'saldoPago', header: 'Saldo', type: 'currency' },
  ];

  readonly actionsPagos: TableAction[] = [
    { id: 'detalle', label: 'Detalle de aplicación', icon: 'fa-solid fa-magnifying-glass', variant: 'info' },
  ];

  readonly columnasMovimientos: TableColumn[] = [
    { key: 'noPago', header: 'No. Pago' },
    { key: 'descripcion', header: 'Descripción' },
    { key: 'fecMovimiento', header: 'Fecha', type: 'date' },
    { key: 'capital', header: 'Capital', type: 'currency' },
    { key: 'interes', header: 'Interés', type: 'currency' },
    { key: 'iva', header: 'IVA', type: 'currency' },
    { key: 'total', header: 'Total', type: 'currency' },
    { key: 'saldoCapital', header: 'Saldo Capital', type: 'currency' },
    { key: 'saldoTotal', header: 'Saldo Total', type: 'currency' },
    { key: 'esRenta', header: 'Renta', type: 'boolean' },
  ];

  readonly actionsMovimientos: TableAction[] = [
    {
      id: 'detalle',
      label: 'Detalle de pagos',
      icon: 'fa-solid fa-magnifying-glass',
      variant: 'info',
      disabledFn: (row: MovimientoItemDto) => !row.idMovimiento || row.idMovimiento <= 0,
    },
  ];

  readonly columnasTablaAmortiza: TableColumn[] = [
    { key: 'noPago', header: 'No. Pago' },
    { key: 'fecInicial', header: 'Fec. Inicial', type: 'date' },
    { key: 'fecVencimiento', header: 'Fec. Vencimiento', type: 'date' },
    { key: 'saldoInicial', header: 'Saldo Inicial', type: 'currency' },
    { key: 'capital', header: 'Capital', type: 'currency' },
    { key: 'interes', header: 'Interés', type: 'currency' },
    { key: 'seguro', header: 'Seguro', type: 'currency' },
    { key: 'iva', header: 'IVA', type: 'currency' },
    { key: 'total', header: 'Total', type: 'currency' },
    { key: 'procesado', header: 'Procesado', type: 'boolean' },
  ];

  readonly detalleFilas = computed(() => this.detalleMovimiento().filter((d) => d.idPago !== -1));
  readonly detalleTotales = computed(() => this.detalleMovimiento().find((d) => d.idPago === -1));

  readonly detallePagoFilas = computed(() => this.detallePago().filter((d) => d.idMovimiento !== -1));
  readonly detallePagoTotales = computed(() => this.detallePago().find((d) => d.idMovimiento === -1));

  constructor() {
    this.wireLoadTabla();
    this.wireLoadDetalleMovimiento();
    this.wireLoadDetallePago();

    this.destroyRef.onDestroy(() => {
      this.bsModalDetalle?.dispose();
      this.bsModalDetallePago?.dispose();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['contrato']) {
      this.cargarDatosDesdeInput();
    }
  }

  setTab(tab: TabActivaAmortizacion): void {
    this.activeTab.set(tab);
    if (tab === 'tabla' && this.tablaAmortiza().length === 0) {
      this.loadTabla$.next(this.tipoTabla());
    }
  }

  loadTabla(): void {
    if (!this.idContrato) return;
    this.loadTabla$.next(this.tipoTabla());
  }

  onTipoTablaChange(): void {
    this.tablaAmortiza.set([]);
    this.loadTabla$.next(this.tipoTabla());
  }

  onActionMovimiento(event: TableActionEvent<MovimientoItemDto>): void {
    if (event.action === 'detalle' && event.row.idMovimiento && event.row.idMovimiento > 0) {
      this.abrirDetalleMovimiento(event.row.idMovimiento);
    }
  }

  abrirDetalleMovimiento(idMovimiento: number): void {
    this.detalleMovimiento.set([]);
    this.isLoadingDetalle.set(true);
    this.bsModalDetalle = new bootstrap.Modal(this.detalleModalEl.nativeElement);
    this.bsModalDetalle.show();
    this.loadDetalleMovimiento$.next(idMovimiento);
  }

  cerrarDetalleMovimiento(): void {
    this.bsModalDetalle?.hide();
  }

  onActionPago(event: TableActionEvent<PagoItemDto>): void {
    if (event.action === 'detalle') {
      this.abrirDetallePago(event.row.idPago!);
    }
  }

  abrirDetallePago(idPago: number): void {
    this.detallePago.set([]);
    this.isLoadingDetallePago.set(true);
    this.bsModalDetallePago = new bootstrap.Modal(this.detallePagoModalEl.nativeElement);
    this.bsModalDetallePago.show();
    this.loadDetallePago$.next(idPago);
  }

  cerrarDetallePago(): void {
    this.bsModalDetallePago?.hide();
  }

  private cargarDatosDesdeInput(): void {
    if (!this.contrato) return;
    this.idContrato = this.contrato.idContrato ?? 0;
    this.pagos.set(this.contrato.pagos ?? []);
    this.movimientos.set(this.contrato.movimientos ?? []);
    this.tablaAmortiza.set(this.contrato.tablaAmortiza ?? []);
    this.activeTab.set('tabla');
    this.tipoTabla.set(1);
  }

  private wireLoadTabla(): void {
    this.loadTabla$.pipe(
      switchMap((tipo) => {
        if (!this.idContrato) return EMPTY;
        this.isLoadingTabla.set(true);
        return this.service.getTablaAmortizacionByTipo(this.idContrato, tipo).pipe(
          timeout(30_000),
          catchError(() => {
            this.isLoadingTabla.set(false);
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.tablaAmortiza.set(res.data ?? []);
      this.isLoadingTabla.set(false);
    });
  }

  private wireLoadDetalleMovimiento(): void {
    this.loadDetalleMovimiento$.pipe(
      switchMap((idMovimiento) =>
        this.service.getMovimientoDetalle(idMovimiento).pipe(
          timeout(30_000),
          catchError(() => {
            this.isLoadingDetalle.set(false);
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.detalleMovimiento.set(res.data?.detalle ?? []);
      this.isLoadingDetalle.set(false);
    });
  }

  private wireLoadDetallePago(): void {
    this.loadDetallePago$.pipe(
      switchMap((idPago) =>
        this.service.getPagoDetalle(idPago).pipe(
          timeout(30_000),
          catchError(() => {
            this.isLoadingDetallePago.set(false);
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.detallePago.set(res.data?.detalle ?? []);
      this.isLoadingDetallePago.set(false);
    });
  }
}
