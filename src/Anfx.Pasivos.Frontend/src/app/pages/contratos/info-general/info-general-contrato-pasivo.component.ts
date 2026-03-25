import { Component, OnInit, OnDestroy, ViewChild, ElementRef, inject, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContratosService } from '../../../../api/services/contratos.service';
import { InfoGeneralContratoPasivoDto } from '../../../../api/models/infoGeneralContratoPasivoDto';
import { PagoItemDto } from '../../../../api/models/pagoItemDto';
import { MovimientoItemDto } from '../../../../api/models/movimientoItemDto';
import { TablaAmortizaItemDto } from '../../../../api/models/tablaAmortizaItemDto';
import { DetallePagoMovimientoDto } from '../../../../api/models/detallePagoMovimientoDto';
import { DetalleMovimientoPagoDto } from '../../../../api/models/detalleMovimientoPagoDto';
import { LayoutService } from '../../../services/layout.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '../../../shared/components/generic-table/table-column.model';
import { ContratoAutocompleteComponent } from "src/app/shared/components/contrato-autocomplete/contrato-autocomplete.component";
import { AutocompleteResultDto } from '../../../../api/models/autocompleteResultDto';
import { CardInfoComponent } from 'src/app/shared/components/card/card-info.component';

export type TabActiva = 'pagos' | 'movimientos' | 'tabla';

@Component({
  selector: 'app-info-general-contrato-pasivo',
  imports: [CommonModule, FormsModule, GenericTableComponent, CardInfoComponent, ContratoAutocompleteComponent],
  templateUrl: './info-general-contrato-pasivo.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InfoGeneralContratoPasivoComponent implements OnInit, OnDestroy {
  @ViewChild('detalleModalEl')     detalleModalEl!: ElementRef<HTMLElement>;
  @ViewChild('detallePagoModalEl') detallePagoModalEl!: ElementRef<HTMLElement>;

  private readonly service       = inject(ContratosService);
  private readonly layoutService = inject(LayoutService);
  private bsModalDetalle?:     any;
  private bsModalDetallePago?: any;

  // Formulario de búsqueda
  contratoBusqueda = signal('');

  // Datos
  info          = signal<InfoGeneralContratoPasivoDto | null>(null);
  pagos         = signal<PagoItemDto[]>([]);
  movimientos   = signal<MovimientoItemDto[]>([]);
  tablaAmortiza = signal<TablaAmortizaItemDto[]>([]);

  // Estado
  isLoading            = signal(false);
  isLoadingTabla       = signal(false);
  isLoadingDetalle     = signal(false);
  isLoadingDetallePago = signal(false);
  errorMsg             = signal<string | null>(null);

  // Modal detalle movimiento
  detalleMovimiento = signal<DetallePagoMovimientoDto[]>([]);

  // Modal detalle pago
  detallePago = signal<DetalleMovimientoPagoDto[]>([]);

  // Tabs
  activeTab = signal<TabActiva>('tabla');
  tipoTabla = signal<number>(1);
  tiposTabla = [
    { value: 1, label: 'Actual' },
    { value: 2, label: 'Detallada' }
  ];

  private idContrato = 0;

  readonly columnasPagos: TableColumn[] = [
    { key: 'idPago',               header: 'ID Pago' },
    { key: 'tipoPago',             header: 'Tipo Pago' },
    { key: 'cuentaBancaria',       header: 'Cuenta Bancaria' },
    { key: 'fecPagoValor',         header: 'Fec. Valor',    type: 'date' },
    { key: 'fecPagoRegistro',      header: 'Fec. Registro', type: 'date' },
    { key: 'montoPago',            header: 'Monto Pago',    type: 'currency' },
    { key: 'montoAplicado',        header: 'Monto Aplicado',type: 'currency' },
    { key: 'montoAplicadoOtros',   header: 'Otros',         type: 'currency' },
    { key: 'saldoPago',            header: 'Saldo',         type: 'currency' },
  ];

  readonly actionsPagos: TableAction[] = [
    { id: 'detalle', label: 'Detalle de aplicación', icon: 'fa-solid fa-magnifying-glass', variant: 'info'},
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

  readonly columnasDetallePago: TableColumn[] = [
    { key: 'tipoPago',       header: 'Tipo Pago' },
    { key: 'cuentaBancaria', header: 'Cuenta Bancaria' },
    { key: 'contrato',       header: 'Contrato' },
    { key: 'fecPagoValor',   header: 'Fec. Valor',    type: 'date' },
    { key: 'fecPagoRegistro',header: 'Fec. Registro', type: 'date' },
    { key: 'montoPago',      header: 'Monto Pago',    type: 'currency' },
    { key: 'capitalPagado',  header: 'Capital',       type: 'currency' },
    { key: 'interesPagado',  header: 'Interés',       type: 'currency' },
    { key: 'ivaPagado',      header: 'IVA',           type: 'currency' },
    { key: 'totalPagado',    header: 'Total',         type: 'currency' },
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

  ngOnInit(): void {
    this.layoutService.setTitle('Información General del Contrato Pasivo');
  }

  onContratoSelected(item: AutocompleteResultDto): void {
    this.contratoBusqueda.set(item.label ?? '');  // ✅ value es la clave de búsqueda
    this.buscar();
  }

  buscar(): void {
    const contrato = this.contratoBusqueda().trim();
    if (!contrato) return;

    this.isLoading.set(true);
    this.errorMsg.set(null);
    this.info.set(null);
    this.pagos.set([]);
    this.movimientos.set([]);
    this.tablaAmortiza.set([]);
    this.activeTab.set('tabla');

    this.service.getInfoGeneralContratoPasivo(contrato).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const d = res.data;
          this.info.set(d);
          this.pagos.set(d.pagos ?? []);
          this.movimientos.set(d.movimientos ?? []);
          this.tablaAmortiza.set(d.tablaAmortiza?? []);
          this.idContrato = d.idContrato??0;
        } else {
          this.errorMsg.set(res.message || 'No se encontró información para el contrato.');
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMsg.set('Error al cargar la información del contrato.');
        this.isLoading.set(false);
      }
    });
  }

  setTab(tab: TabActiva): void {
    this.activeTab.set(tab);
    if (tab === 'tabla' && this.tablaAmortiza().length === 0) {
      this.loadTabla();
    }
  }

  loadTabla(): void {
    this.isLoadingTabla.set(true);
    this.service.getTablaAmortizacionByTipo(this.idContrato, this.tipoTabla()).subscribe({
      next: (res) => {
        this.tablaAmortiza.set(res.data ?? []);
        this.isLoadingTabla.set(false);
      },
      error: () => {
        this.isLoadingTabla.set(false);
      }
    });
  }

  onTipoTablaChange(): void {
    this.tablaAmortiza.set([]);
    this.loadTabla();
  }

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
      }
    });
  }

  cerrarDetalleMovimiento(): void {
    this.bsModalDetalle?.hide();
  }

  readonly detalleFilas    = computed(() => this.detalleMovimiento().filter(d => d.idPago !== -1));
  readonly detalleTotales  = computed(() => this.detalleMovimiento().find(d => d.idPago === -1));

  readonly detallePagoFilas    = computed(() => this.detallePago().filter(d => d.idMovimiento !== -1));
  readonly detallePagoTotales  = computed(() => this.detallePago().find(d => d.idMovimiento === -1));

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
      }
    });
  }

  cerrarDetallePago(): void {
    this.bsModalDetallePago?.hide();
  }

  ngOnDestroy(): void {
    this.bsModalDetalle?.dispose();
    this.bsModalDetallePago?.dispose();
  }
}
