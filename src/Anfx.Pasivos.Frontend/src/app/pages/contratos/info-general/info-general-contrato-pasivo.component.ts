import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContratosService } from '../../../../api/services/contratos.service';
import { InfoGeneralContratoPasivoDto } from '../../../../api/models/infoGeneralContratoPasivoDto';
import { PagoItemDto } from '../../../../api/models/pagoItemDto';
import { MovimientoItemDto } from '../../../../api/models/movimientoItemDto';
import { TablaAmortizaItemDto } from '../../../../api/models/tablaAmortizaItemDto';
import { LayoutService } from '../../../services/layout.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn } from '../../../shared/components/generic-table/table-column.model';
import { SearchInputComponent } from '../../../shared/components/search-input/search-input.component';

type TabActiva = 'pagos' | 'movimientos' | 'tabla';

@Component({
  selector: 'app-info-general-contrato-pasivo',
  standalone: true,
  imports: [CommonModule, FormsModule, GenericTableComponent, SearchInputComponent],
  templateUrl: './info-general-contrato-pasivo.component.html'
})
export class InfoGeneralContratoPasivoComponent implements OnInit {
  private service       = inject(ContratosService);
  private layoutService = inject(LayoutService);

  // Formulario de búsqueda
  contratoBusqueda = signal('');

  // Datos
  info          = signal<InfoGeneralContratoPasivoDto | null>(null);
  pagos         = signal<PagoItemDto[]>([]);
  movimientos   = signal<MovimientoItemDto[]>([]);
  tablaAmortiza = signal<TablaAmortizaItemDto[]>([]);

  // Estado
  isLoading      = signal(false);
  isLoadingTabla = signal(false);
  errorMsg       = signal<string | null>(null);

  // Tabs
  activeTab = signal<TabActiva>('pagos');
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

  onSearch(value: string): void {
    this.contratoBusqueda.set(value);
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
    this.activeTab.set('pagos');

    this.service.getInfoGeneralContratoPasivo(contrato).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const d = res.data;
          this.info.set(d);
          this.pagos.set(d.pagos ?? []);
          this.movimientos.set(d.movimientos ?? []);
          this.idContrato = d.pagos?.[0]?.idContratoPasivo ?? parseInt(contrato, 10) ?? 0;
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
}
