import {
  Component,
  Input,
  OnInit,
  OnDestroy,
  OnChanges,
  SimpleChanges,
  ViewChild,
  ElementRef,
  inject,
  signal,
  computed,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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

@Component({
  selector: 'app-contrato-amortizacion-pagos',
  standalone: true,
  imports: [CommonModule, FormsModule, GenericTableComponent],
  templateUrl: './contrato-amortizacion-pagos.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContratoAmortizacionPagosComponent implements OnInit, OnChanges, OnDestroy {
  @Input() contrato: InfoGeneralContratoPasivoDto | null = null;

  @ViewChild('detalleModalEl')     detalleModalEl!: ElementRef<HTMLElement>;
  @ViewChild('detallePagoModalEl') detallePagoModalEl!: ElementRef<HTMLElement>;

  private readonly service = inject(ContratosService);
  private bsModalDetalle?:     any;
  private bsModalDetallePago?: any;

  // Datos
  pagos         = signal<PagoItemDto[]>([]);
  movimientos   = signal<MovimientoItemDto[]>([]);
  tablaAmortiza = signal<TablaAmortizaItemDto[]>([]);

  // Estado
  isLoadingTabla       = signal(false);
  isLoadingDetalle     = signal(false);
  isLoadingDetallePago = signal(false);

  // Modal detalle movimiento
  detalleMovimiento = signal<DetallePagoMovimientoDto[]>([]);

  // Modal detalle pago
  detallePago = signal<DetalleMovimientoPagoDto[]>([]);

  // Tabs
  activeTab = signal<TabActivaAmortizacion>('tabla');
  tipoTabla = signal<number>(1);
  tiposTabla = [
    { value: 1, label: 'Actual' },
    { value: 2, label: 'Detallada' },
  ];

  private idContrato = 0;

  // ── Columnas ────────────────────────────────────────────────────────────────

  readonly columnasPagos: TableColumn[] = [
    { key: 'idPago',             header: 'ID Pago' },
    { key: 'tipoPago',           header: 'Tipo Pago' },
    { key: 'cuentaBancaria',     header: 'Cuenta Bancaria' },
    { key: 'fecPagoValor',       header: 'Fec. Valor',     type: 'date' },
    { key: 'fecPagoRegistro',    header: 'Fec. Registro',  type: 'date' },
    { key: 'montoPago',          header: 'Monto Pago',     type: 'currency' },
    { key: 'montoAplicado',      header: 'Monto Aplicado', type: 'currency' },
    { key: 'montoAplicadoOtros', header: 'Otros',          type: 'currency' },
    { key: 'saldoPago',          header: 'Saldo',          type: 'currency' },
  ];

  readonly actionsPagos: TableAction[] = [
    { id: 'detalle', label: 'Detalle de aplicación', icon: 'fa-solid fa-magnifying-glass', variant: 'info' },
  ];

  readonly columnasMovimientos: TableColumn[] = [
    { key: 'noPago',       header: 'No. Pago' },
    { key: 'descripcion',  header: 'Descripción' },
    { key: 'fecMovimiento',header: 'Fecha',         type: 'date' },
    { key: 'capital',      header: 'Capital',       type: 'currency' },
    { key: 'interes',      header: 'Interés',       type: 'currency' },
    { key: 'iva',          header: 'IVA',           type: 'currency' },
    { key: 'total',        header: 'Total',         type: 'currency' },
    { key: 'saldoCapital', header: 'Saldo Capital', type: 'currency' },
    { key: 'saldoTotal',   header: 'Saldo Total',   type: 'currency' },
    { key: 'esRenta',      header: 'Renta',         type: 'boolean' },
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
    { key: 'noPago',         header: 'No. Pago' },
    { key: 'fecInicial',     header: 'Fec. Inicial',     type: 'date' },
    { key: 'fecVencimiento', header: 'Fec. Vencimiento', type: 'date' },
    { key: 'saldoInicial',   header: 'Saldo Inicial',    type: 'currency' },
    { key: 'capital',        header: 'Capital',          type: 'currency' },
    { key: 'interes',        header: 'Interés',          type: 'currency' },
    { key: 'seguro',         header: 'Seguro',           type: 'currency' },
    { key: 'iva',            header: 'IVA',              type: 'currency' },
    { key: 'total',          header: 'Total',            type: 'currency' },
    { key: 'procesado',      header: 'Procesado',        type: 'boolean' },
  ];

  // ── Computed ─────────────────────────────────────────────────────────────────

  readonly detalleFilas   = computed(() => this.detalleMovimiento().filter(d => d.idPago !== -1));
  readonly detalleTotales = computed(() => this.detalleMovimiento().find(d => d.idPago === -1));

  readonly detallePagoFilas   = computed(() => this.detallePago().filter(d => d.idMovimiento !== -1));
  readonly detallePagoTotales = computed(() => this.detallePago().find(d => d.idMovimiento === -1));

  // ── Lifecycle ────────────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.cargarDatosDesdeInput();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['contrato']) {
      this.cargarDatosDesdeInput();
    }
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

  // ── Tabs ─────────────────────────────────────────────────────────────────────

  setTab(tab: TabActivaAmortizacion): void {
    this.activeTab.set(tab);
    if (tab === 'tabla' && this.tablaAmortiza().length === 0) {
      this.loadTabla();
    }
  }

  loadTabla(): void {
    if (!this.idContrato) return;
    this.isLoadingTabla.set(true);
    this.service.getTablaAmortizacionByTipo(this.idContrato, this.tipoTabla()).subscribe({
      next: (res) => {
        this.tablaAmortiza.set(res.data ?? []);
        this.isLoadingTabla.set(false);
      },
      error: () => {
        this.isLoadingTabla.set(false);
      },
    });
  }

  onTipoTablaChange(): void {
    this.tablaAmortiza.set([]);
    this.loadTabla();
  }

  // ── Acciones Movimientos ─────────────────────────────────────────────────────

  onActionMovimiento(event: TableActionEvent<MovimientoItemDto>): void {
    if (event.action === 'detalle' && event.row.idMovimiento && event.row.idMovimiento > 0) {
      this.abrirDetalleMovimiento(event.row.idMovimiento);
    }
  }

  abrirDetalleMovimiento(idMovimiento: number): void {
    this.detalleMovimiento.set([]);
    this.isLoadingDetalle.set(true);
    this.bsModalDetalle = new (globalThis as any).bootstrap.Modal(this.detalleModalEl.nativeElement);
    this.bsModalDetalle.show();

    this.service.getMovimientoDetalle(idMovimiento).subscribe({
      next: (res) => {
        this.detalleMovimiento.set(res.data?.detalle ?? []);
        this.isLoadingDetalle.set(false);
      },
      error: () => {
        this.isLoadingDetalle.set(false);
      },
    });
  }

  cerrarDetalleMovimiento(): void {
    this.bsModalDetalle?.hide();
  }

  // ── Acciones Pagos ───────────────────────────────────────────────────────────

  onActionPago(event: TableActionEvent<PagoItemDto>): void {
    if (event.action === 'detalle') {
      this.abrirDetallePago(event.row.idPago!);
    }
  }

  abrirDetallePago(idPago: number): void {
    this.detallePago.set([]);
    this.isLoadingDetallePago.set(true);
    this.bsModalDetallePago = new (globalThis as any).bootstrap.Modal(this.detallePagoModalEl.nativeElement);
    this.bsModalDetallePago.show();

    this.service.getPagoDetalle(idPago).subscribe({
      next: (res) => {
        this.detallePago.set(res.data?.detalle ?? []);
        this.isLoadingDetallePago.set(false);
      },
      error: () => {
        this.isLoadingDetallePago.set(false);
      },
    });
  }

  cerrarDetallePago(): void {
    this.bsModalDetallePago?.hide();
  }

  // ── Destroy ──────────────────────────────────────────────────────────────────

  ngOnDestroy(): void {
    this.bsModalDetalle?.dispose();
    this.bsModalDetallePago?.dispose();
  }
}
