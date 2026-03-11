import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportesService } from '@api/services/reportes.service';
import { SelectListsService } from '@api/services/selectLists.service';
import { SelectItemDto } from '@api/models/selectItemDto';
import { CarteraReporteDto } from '@api/models/carteraReporteDto';
import { DashboardResponse } from '@api/models/dashboardResponse';
import { HighchartsChartComponent } from 'highcharts-angular';
import * as Highcharts from 'highcharts';
import { SelectItemDtoListApiResponseDto } from '@api/models/selectItemDtoListApiResponseDto';
import { CarteraReporteDtoPagedResultDtoApiResponseDto } from '../../../../api/models/carteraReporteDtoPagedResultDtoApiResponseDto';
import { DashboardResponseApiResponseDto } from '@api/models/dashboardResponseApiResponseDto';
import { CarteraMensualDto } from '@api/models/carteraMensualDto';
import { CarteraDto } from '@api/models/carteraDto';
import { LayoutService } from 'src/app/services/layout.service';

@Component({
  selector: 'app-dashboard-reportes',
  standalone: true,
  imports: [CommonModule, FormsModule, HighchartsChartComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  Highcharts: typeof Highcharts = Highcharts;
  private layoutService = inject(LayoutService);
  // Select Lists
  fondeadores = signal<SelectItemDto[]>([]);
  contratosPasivos = signal<SelectItemDto[]>([]);
  contratosActivos = signal<SelectItemDto[]>([]);
  saldosOptions = [
    { value: 1, label: 'Por Vencer' },
    { value: 2, label: 'Vencido' },
  ];

  // Filters
  selectedFondeador = signal<number | undefined>(undefined);
  selectedContratoPasivo = signal<number | undefined>(undefined);
  selectedContratoActivo = signal<number | undefined>(undefined);
  selectedSaldo = signal<number | undefined>(undefined);

  // Table Data
  carteraActiva = signal<CarteraReporteDto[]>([]);
  carteraPasiva = signal<CarteraReporteDto[]>([]);
  isLoading = signal<boolean>(false);

  // Pagination
  pageActiva = 1;
  pageSizeActiva = 10;
  totalActiva = 0;

  pagePasiva = 1;
  pageSizePasiva = 10;
  totalPasiva = 0;

  // Chart Options
  pieChartActivaOptions: Highcharts.Options = {
    chart: { type: 'pie' },
    title: { text: 'Cartera Activa' },
    series: [{ name: 'Monto', type: 'pie', data: [] }],
  };
  pieChartPasivaOptions: Highcharts.Options = {
    chart: { type: 'pie' },
    title: { text: 'Cartera Pasiva' },
    series: [{ name: 'Monto', type: 'pie', data: [] }],
  };
  barChartActivaOptions: Highcharts.Options = {
    chart: { type: 'column' },
    title: { text: 'Interés y Capital Activo' },
    series: [],
  };
  barChartPasivaOptions: Highcharts.Options = {
    chart: { type: 'column' },
    title: { text: 'Interés y Capital Pasivo' },
    series: [],
  };
  relationChartOptions: Highcharts.Options = {
    chart: { type: 'column' },
    title: { text: 'Relación Activo vs Pasivo' },
    series: [],
  };

  showTables = signal(false);
  showCharts = signal(false);

  constructor(
    private reportesService: ReportesService,
    private selectListsService: SelectListsService,
  ) {
    // Effect to reload dependent dropdowns
    effect(() => {
      const fondeadorId = this.selectedFondeador();
      if (fondeadorId) {
        this.loadContratosPasivos(fondeadorId);
      } else {
        this.contratosPasivos.set([]);
        this.selectedContratoPasivo.set(undefined);
      }
    });

    effect(() => {
      const pasivoId = this.selectedContratoPasivo();
      if (pasivoId) {
        this.loadContratosActivos(pasivoId);
      } else {
        this.contratosActivos.set([]);
        this.selectedContratoActivo.set(undefined);
      }
    });
  }
  ngOnInit(): void {
    this.layoutService.setTitle('Monitor de Cartera Pasiva');
    this.loadFondeadores();
    this.mostrarGraficos();
  }

  loadFondeadores() {
    this.selectListsService
      .getFondeadoresSelectList()
      .subscribe((res: SelectItemDtoListApiResponseDto) => {
        if (res.data) this.fondeadores.set(res.data);
      });
  }

  loadContratosPasivos(idFondeador: number) {
    this.selectListsService
      .getContratosPasivosPorFondeador(idFondeador)
      .subscribe((res: SelectItemDtoListApiResponseDto) => {
        if (res.data) this.contratosPasivos.set(res.data);
      });
  }

  loadContratosActivos(idContratoPasivo: number) {
    this.selectListsService
      .getContratosActivosPorPasivo(idContratoPasivo)
      .subscribe((res: SelectItemDtoListApiResponseDto) => {
        if (res.data) this.contratosActivos.set(res.data);
      });
  }

  obtenerDetalle() {
    this.showTables.set(true);
    this.showCharts.set(false);
    this.loadCarteraActiva();
    this.loadCarteraPasiva();
  }

  loadCarteraActiva() {
    this.reportesService
      .getCarteraPorVencer(
        this.pageActiva,
        this.pageSizeActiva,
        undefined,
        this.selectedFondeador(),
        this.selectedContratoPasivo(),
        this.selectedContratoActivo(),
        this.selectedSaldo(),
      )
      .subscribe((res: CarteraReporteDtoPagedResultDtoApiResponseDto) => {
        if (res.data && res.data.results) {
          this.carteraActiva.set(res.data.results);
          this.totalActiva = res.data.totalCount || 0;
        }
      });
  }

  loadCarteraPasiva() {
    this.reportesService
      .getCarteraPasivaPorVencer(
        this.pagePasiva,
        this.pageSizePasiva,
        undefined,
        this.selectedFondeador(),
        this.selectedContratoPasivo(),
        this.selectedContratoActivo(),
        this.selectedSaldo(),
      )
      .subscribe((res: CarteraReporteDtoPagedResultDtoApiResponseDto) => {
        if (res.data && res.data.results) {
          this.carteraPasiva.set(res.data.results);
          this.totalPasiva = res.data.totalCount || 0;
        }
      });
  }

  mostrarGraficos() {
    this.showTables.set(false);
    this.showCharts.set(true);
    this.isLoading.set(true);
    this.reportesService
      .getDashboard(
        this.selectedFondeador(),
        this.selectedContratoPasivo(),
        this.selectedContratoActivo(),
        this.selectedSaldo(),
      )
      .subscribe((res: DashboardResponseApiResponseDto) => {
        if (res.data) {
          this.setupCharts(res.data);
        }
        this.isLoading.set(false);
      });
  }

  setupCharts(data: DashboardResponse) {
    console.log(data);
    this.pieChartActivaOptions = this.getPieChartOptions('Cartera Activa', data.activos!);
    this.pieChartPasivaOptions = this.getPieChartOptions('Cartera Pasiva', data.pasivos!);
    this.barChartActivaOptions = this.getBarChartOptions(
      'Interés y Capital Activo',
      data.activosMensual || [],
    );
    this.barChartPasivaOptions = this.getBarChartOptions(
      'Interés y Capital Pasivo',
      data.pasivosMensual || [],
    );
    this.relationChartOptions = this.getRelationChartOptions(data);
  }

  getPieChartOptions(title: string, data: CarteraDto): Highcharts.Options {
    return {
      chart: { type: 'pie' },
      title: { text: title },
      series: [
        {
          name: 'Monto',
          type: 'pie',
          data: [
            { name: 'Capital', y: data?.capital || 0 },
            { name: 'Interés', y: data?.interes || 0 },
          ],
        },
      ],
    };
  }

  getBarChartOptions(title: string, data: CarteraMensualDto[]): Highcharts.Options {
    const categories = data.map((d) => (d.fecIni ? new Date(d.fecIni).toLocaleDateString() : ''));
    const capitalData = data.map((d) => d.capital || 0);
    const interesData = data.map((d) => d.interes || 0);

    return {
      chart: { type: 'column' },
      title: { text: title },
      xAxis: { categories: categories },
      yAxis: { title: { text: 'Monto' } },
      series: [
        { name: 'Capital', type: 'column', data: capitalData },
        { name: 'Interés', type: 'column', data: interesData },
      ],
    };
  }

  getRelationChartOptions(data: DashboardResponse): Highcharts.Options {
    return {
      chart: { type: 'column' },
      title: { text: 'Relación Activo vs Pasivo' },
      xAxis: { categories: ['Total'] },
      series: [
        {
          name: 'Activo',
          type: 'column',
          data: [(data.activos?.capital || 0) + (data.activos?.interes || 0)],
        },
        {
          name: 'Pasivo',
          type: 'column',
          data: [(data.pasivos?.capital || 0) + (data.pasivos?.interes || 0)],
        },
      ],
    };
  }
}
