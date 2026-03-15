import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContratosService } from '../../../../api/services/contratos.service';
import { InfoGeneralContratoPasivoDto } from '../../../../api/models/infoGeneralContratoPasivoDto';
import { PagoItemDto } from '../../../../api/models/pagoItemDto';
import { MovimientoItemDto } from '../../../../api/models/movimientoItemDto';
import { TablaAmortizaItemDto } from '../../../../api/models/tablaAmortizaItemDto';
import { LayoutService } from '../../../services/layout.service';

type TabActiva = 'pagos' | 'movimientos' | 'tabla';

@Component({
  selector: 'app-info-general-contrato-pasivo',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

  ngOnInit(): void {
    this.layoutService.setTitle('Información General del Contrato Pasivo');
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
