import { Component, OnInit, inject, signal, computed, DestroyRef } from '@angular/core';
import { of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportesService } from '@api/services/reportes.service';
import { SelectListsService } from '@api/services/selectLists.service';
import { SelectItemDto } from '@api/models/selectItemDto';
import { CarteraReporteDto } from '@api/models/carteraReporteDto';
import { DashboardResponse } from '@api/models/dashboardResponse';
import { HighchartsChartComponent } from 'highcharts-angular';
import type { Options as HighchartsOptions } from 'highcharts';
import { CarteraMensualDto } from '@api/models/carteraMensualDto';
import { CarteraDto } from '@api/models/carteraDto';
import { LayoutService } from 'src/app/services/layout.service';
import { UtilsService } from 'src/app/services/utils.service';
import { CardInfoComponent } from 'src/app/shared/components/card/card-info.component';

@Component({
  selector: 'app-dashboard-reportes',
  standalone: true,
  imports: [CommonModule, FormsModule, HighchartsChartComponent, CardInfoComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  private readonly layoutService      = inject(LayoutService);
  private readonly reportesService    = inject(ReportesService);
  private readonly selectListsService = inject(SelectListsService);
  private readonly utilsService       = inject(UtilsService);
  private readonly destroyRef         = inject(DestroyRef);

  // ── Select Lists ─────────────────────────────────────────────
  fondeadores      = signal<SelectItemDto[]>([]);
  contratosPasivos = signal<SelectItemDto[]>([]);
  contratosActivos = signal<SelectItemDto[]>([]);
  saldosOptions = [
    { value: 1, label: 'Por Vencer' },
    { value: 2, label: 'Vencido' },
  ];

  // ── Filtros ──────────────────────────────────────────────────
  selectedFondeador      = signal<number | undefined>(undefined);
  selectedContratoPasivo = signal<number | undefined>(undefined);
  selectedContratoActivo = signal<number | undefined>(undefined);
  selectedSaldo          = signal<number | undefined>(undefined);

  // ── Estado ───────────────────────────────────────────────────
  carteraActiva    = signal<CarteraReporteDto[]>([]);
  carteraPasiva    = signal<CarteraReporteDto[]>([]);
  isLoading        = signal(false);   // gráficas
  isLoadingActiva  = signal(false);   // tabla activa
  isLoadingPasiva  = signal(false);   // tabla pasiva
  showTables       = signal(false);
  showCharts       = signal(false);

  readonly isBusy = computed(() =>
    this.isLoading() || this.isLoadingActiva() || this.isLoadingPasiva()
  );

  // ── Paginación ───────────────────────────────────────────────
  pageActiva     = 1;
  pageSizeActiva = 10;
  totalActiva    = signal(0);

  pagePasiva     = 1;
  pageSizePasiva = 10;
  totalPasiva    = signal(0);

  readonly totalPagesActiva = computed(() =>
    Math.max(1, Math.ceil(this.totalActiva() / this.pageSizeActiva))
  );
  readonly totalPagesPasiva = computed(() =>
    Math.max(1, Math.ceil(this.totalPasiva() / this.pageSizePasiva))
  );

  // ── Totales de tabla (computed) ───────────────────────────────
  readonly sumCapitalActiva = computed(() =>
    this.carteraActiva().reduce((s, r) => s + (r.capital ?? 0), 0)
  );
  readonly sumInteresActiva = computed(() =>
    this.carteraActiva().reduce((s, r) => s + (r.interes ?? 0), 0)
  );
  readonly sumCapitalPasiva = computed(() =>
    this.carteraPasiva().reduce((s, r) => s + (r.capital ?? 0), 0)
  );
  readonly sumInteresPasiva = computed(() =>
    this.carteraPasiva().reduce((s, r) => s + (r.interes ?? 0), 0)
  );

  // ── Chart Options ─────────────────────────────────────────────
  pieChartActivaOptions: HighchartsOptions = {};
  pieChartPasivaOptions: HighchartsOptions = {};
  barChartActivaOptions: HighchartsOptions = {};
  barChartPasivaOptions: HighchartsOptions = {};
  relationChartOptions:  HighchartsOptions = {};


  constructor() {
    // Fondeador → contratos pasivos (switchMap cancela request anterior)
    toObservable(this.selectedFondeador).pipe(
      takeUntilDestroyed(this.destroyRef),
      switchMap(id => {
        this.selectedContratoPasivo.set(undefined);
        this.contratosPasivos.set([]);
        if (!id) return of(null);
        return this.selectListsService.getContratosPasivosPorFondeador(id);
      })
    ).subscribe(res => {
      if (res?.data) this.contratosPasivos.set(res.data);
    });

    // Contrato Pasivo → contratos activos
    toObservable(this.selectedContratoPasivo).pipe(
      takeUntilDestroyed(this.destroyRef),
      switchMap(id => {
        this.selectedContratoActivo.set(undefined);
        this.contratosActivos.set([]);
        if (!id) return of(null);
        return this.selectListsService.getContratosActivosPorPasivo(id);
      })
    ).subscribe(res => {
      if (res?.data) this.contratosActivos.set(res.data);
    });
  }

  ngOnInit(): void {
    this.layoutService.setTitle('Monitor de Cartera Pasiva');
    this.loadFondeadores();
    this.mostrarGraficos();
  }

  // ── Filtros ──────────────────────────────────────────────────

  limpiarFiltros() {
    this.selectedFondeador.set(undefined);
    this.selectedContratoPasivo.set(undefined);
    this.selectedContratoActivo.set(undefined);
    this.selectedSaldo.set(undefined);
  }

  // ── Carga de select lists ────────────────────────────────────

  private loadFondeadores() {
    this.selectListsService.getFondeadoresSelectList()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(res => {
        if (res.data) this.fondeadores.set(res.data);
      });
  }

  // ── Vista: Tablas ─────────────────────────────────────────────

  obtenerDetalle() {
    this.showTables.set(true);
    this.showCharts.set(false);
    this.pageActiva = 1;
    this.pagePasiva = 1;
    this.loadCarteraActiva();
    this.loadCarteraPasiva();
  }

  nextPageActiva() {
    if (this.pageActiva < this.totalPagesActiva()) {
      this.pageActiva++;
      this.loadCarteraActiva();
    }
  }

  prevPageActiva() {
    if (this.pageActiva > 1) {
      this.pageActiva--;
      this.loadCarteraActiva();
    }
  }

  nextPagePasiva() {
    if (this.pagePasiva < this.totalPagesPasiva()) {
      this.pagePasiva++;
      this.loadCarteraPasiva();
    }
  }

  prevPagePasiva() {
    if (this.pagePasiva > 1) {
      this.pagePasiva--;
      this.loadCarteraPasiva();
    }
  }

  private loadCarteraActiva() {
    this.isLoadingActiva.set(true);
    this.reportesService.getCarteraPorVencer(
      this.pageActiva, this.pageSizeActiva, undefined,
      this.selectedFondeador(), this.selectedContratoPasivo(),
      this.selectedContratoActivo(), this.selectedSaldo(),
    ).pipe(takeUntilDestroyed(this.destroyRef))
     .subscribe({
      next: res => {
        if (res.data?.results) {
          this.carteraActiva.set(res.data.results);
          this.totalActiva.set(res.data.totalCount ?? 0);
        }
        this.isLoadingActiva.set(false);
      },
      error: () => {
        this.isLoadingActiva.set(false);
        this.utilsService.showNotification('Error', 'Error al cargar cartera activa', 'error');
      }
    });
  }

  private loadCarteraPasiva() {
    this.isLoadingPasiva.set(true);
    this.reportesService.getCarteraPasivaPorVencer(
      this.pagePasiva, this.pageSizePasiva, undefined,
      this.selectedFondeador(), this.selectedContratoPasivo(),
      this.selectedContratoActivo(), this.selectedSaldo(),
    ).pipe(takeUntilDestroyed(this.destroyRef))
     .subscribe({
      next: res => {
        if (res.data?.results) {
          this.carteraPasiva.set(res.data.results);
          this.totalPasiva.set(res.data.totalCount ?? 0);
        }
        this.isLoadingPasiva.set(false);
      },
      error: () => {
        this.isLoadingPasiva.set(false);
        this.utilsService.showNotification('Error', 'Error al cargar cartera pasiva', 'error');
      }
    });
  }

  // ── Vista: Gráficas ───────────────────────────────────────────

  mostrarGraficos() {
    this.showTables.set(false);
    this.showCharts.set(false);   // ocultar mientras carga
    this.isLoading.set(true);
    this.reportesService.getDashboard(
      this.selectedFondeador(),
      this.selectedContratoPasivo(),
      this.selectedContratoActivo(),
      this.selectedSaldo(),
    ).pipe(takeUntilDestroyed(this.destroyRef))
     .subscribe({
      next: res => {
        if (res.data) {
          this.buildCharts(res.data);
          this.showCharts.set(true);   // mostrar solo cuando hay datos
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.utilsService.showNotification('Error', 'Error al cargar el dashboard', 'error');
      }
    });
  }

  private buildCharts(data: DashboardResponse) {
    this.pieChartActivaOptions = this.buildPieChart('Cartera Activa',  data.activos!);
    this.pieChartPasivaOptions = this.buildPieChart('Cartera Pasiva',  data.pasivos!);
    this.barChartActivaOptions = this.buildBarChart('Evolución Mensual — Activo', data.activosMensual ?? []);
    this.barChartPasivaOptions = this.buildBarChart('Evolución Mensual — Pasivo', data.pasivosMensual ?? []);
    this.relationChartOptions  = this.buildRelationChart(data);

  }

  // ── Constructores de opciones Highcharts ─────────────────────

  private buildPieChart(title: string, data: CarteraDto): HighchartsOptions {
    const capital = data?.capital ?? 0;
    const interes = data?.interes ?? 0;
    return {
      chart:   { type: 'pie' },
      credits: { enabled: false },
      title:   { text: title, style: { fontSize: '14px', fontWeight: '600' } },
      subtitle: {
        text: `Total: ${this.formatMXN(capital + interes)}`,
        style: { fontSize: '12px', color: '#6b7280' },
      },
      tooltip: {
        pointFormat: '<b>{point.name}</b>: <b>${point.y:,.2f} MXN</b><br/>({point.percentage:.1f}%)',
      },
      plotOptions: {
        pie: {
          innerSize: '50%',
          dataLabels: {
            enabled: true,
            format: '<b>{point.name}</b><br/>{point.percentage:.1f}%',
            distance: 15,
            style: { fontSize: '12px' },
          },
        },
      },
      series: [{
        name: 'Monto',
        type: 'pie',
        data: [
          { name: 'Capital', y: capital, color: '#1d6cf5' },
          { name: 'Interés', y: interes, color: '#27ae60' },
        ],
      }],
    };
  }

  private buildBarChart(title: string, data: CarteraMensualDto[]): HighchartsOptions {
    const categories = data.map(d => {
      if (!d.fecIni) return '';
      // Parsear sin conversión de zona horaria
      const [y, m] = d.fecIni.toString().split('T')[0].split('-');
      return new Intl.DateTimeFormat('es-MX', { month: 'short', year: 'numeric' })
        .format(new Date(+y, +m - 1, 1));
    });

    return {
      chart:   { type: 'column' },
      credits: { enabled: false },
      title:   { text: title, style: { fontSize: '14px', fontWeight: '600' } },
      xAxis:   { categories },
      yAxis: {
        title: { text: 'Monto (MXN)' },
        labels: {
          formatter: function () {
            const v = this.value as number;
            if (Math.abs(v) >= 1_000_000) return `$${(v / 1_000_000).toFixed(1)}M`;
            if (Math.abs(v) >= 1_000)     return `$${(v / 1_000).toFixed(0)}K`;
            return `$${v}`;
          },
        },
      },
      tooltip: { valuePrefix: '$', valueDecimals: 2, valueSuffix: ' MXN', shared: true },
      plotOptions: {
        column: { borderRadius: 4, groupPadding: 0.1 },
      },
      series: [
        { name: 'Capital', type: 'column', data: data.map(d => d.capital ?? 0), color: '#1d6cf5' },
        { name: 'Interés', type: 'column', data: data.map(d => d.interes ?? 0), color: '#27ae60' },
      ],
    };
  }

  private buildRelationChart(data: DashboardResponse): HighchartsOptions {
    const aC = data.activos?.capital ?? 0;
    const aI = data.activos?.interes ?? 0;
    const pC = data.pasivos?.capital ?? 0;
    const pI = data.pasivos?.interes ?? 0;

    return {
      chart:   { type: 'bar' },
      credits: { enabled: false },
      title:   { text: 'Relación Activo vs Pasivo', style: { fontSize: '14px', fontWeight: '600' } },
      xAxis:   { categories: ['Capital', 'Interés', 'Total'] },
      yAxis: {
        title: { text: 'Monto (MXN)' },
        labels: {
          formatter: function () {
            const v = this.value as number;
            if (Math.abs(v) >= 1_000_000) return `$${(v / 1_000_000).toFixed(1)}M`;
            if (Math.abs(v) >= 1_000)     return `$${(v / 1_000).toFixed(0)}K`;
            return `$${v}`;
          },
        },
      },
      tooltip: { valuePrefix: '$', valueDecimals: 2, valueSuffix: ' MXN', shared: true },
      plotOptions: {
        bar: { borderRadius: 4, groupPadding: 0.1 },
      },
      series: [
        { name: 'Activo', type: 'bar', data: [aC, aI, aC + aI], color: '#1d6cf5' },
        { name: 'Pasivo', type: 'bar', data: [pC, pI, pC + pI], color: '#e03232' },
      ],
    };
  }

  // ── Helpers ───────────────────────────────────────────────────

  private formatMXN(value: number): string {
    return new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN' }).format(value);
  }
}
