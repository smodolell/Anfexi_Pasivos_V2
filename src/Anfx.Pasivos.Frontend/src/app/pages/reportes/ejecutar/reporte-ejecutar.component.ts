import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { ReportesService } from '@api/services/reportes.service';
import { Configuration } from '@api/configuration';
import { CardComponent } from '@shared/components/card/card.component';
import { SelectReporteDto } from '@api/models/selectReporteDto';
import { ReporteExecuteDto } from '@api/models/reporteExecuteDto';
import { ReporteExecuteParametroDto } from '@api/models/reporteExecuteParametroDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';

const INPUT_TEXTBOX      = 1;
const INPUT_CHECKBOX     = 2;
const INPUT_DATEPICKER   = 3;
const INPUT_DROPDOWN     = 4;
const INPUT_USUARIO_AUTO = 12;

@Component({
  selector: 'app-reporte-ejecutar',
  standalone: true,
  imports: [CommonModule, FormsModule, CardComponent],
  templateUrl: './reporte-ejecutar.component.html',
})
export class ReporteEjecutarComponent implements OnInit {
  private readonly reportesService = inject(ReportesService);
  private readonly utilsService = inject(UtilsService);
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(Configuration);

  reporteOptions = signal<SelectReporteDto[]>([]);
  selectedReporteId: number | null = null;
  configuracion = signal<ReporteExecuteDto | null>(null);
  parametros = signal<ReporteExecuteParametroDto[]>([]);
  guardarHistorial = true;

  isLoadingReportes = signal(false);
  isLoadingConfig = signal(false);
  isEjecutando = signal(false);

  readonly INPUT_TEXTBOX      = INPUT_TEXTBOX;
  readonly INPUT_CHECKBOX     = INPUT_CHECKBOX;
  readonly INPUT_DATEPICKER   = INPUT_DATEPICKER;
  readonly INPUT_DROPDOWN     = INPUT_DROPDOWN;
  readonly INPUT_USUARIO_AUTO = INPUT_USUARIO_AUTO;

  ngOnInit() {
    this.loadReportes();
  }

  onReporteChange() {
    if (!this.selectedReporteId) {
      this.configuracion.set(null);
      this.parametros.set([]);
      return;
    }
    this.loadConfiguracion(+this.selectedReporteId);
  }

  updateParamValue(index: number, field: 'value' | 'valueBoolean' | 'valueDateTime', value: any) {
    const params = [...this.parametros()];
    params[index] = { ...params[index], [field]: value };
    this.parametros.set(params);
  }

  canEjecutar(): boolean {
    return !!this.selectedReporteId && !this.isEjecutando();
  }

  onEjecutar() {
    const config = this.configuracion();
    if (!config || !this.selectedReporteId) return;

    this.isEjecutando.set(true);

    const dto: ReporteExecuteDto = {
      ...config,
      reporteId:  +this.selectedReporteId,
      parametros: this.parametros(),
    };

    const url = `${this.apiConfig.basePath}/api/reportes/reporte/ejecutar?guardarArchivo=${this.guardarHistorial}`;
    this.http
      .post(url, dto, { observe: 'response', responseType: 'blob' })
      .subscribe({
        next: (res: HttpResponse<Blob>) => {
          this.isEjecutando.set(false);
          this.triggerDownload(res);
          this.utilsService.showNotification('Éxito', 'Reporte generado correctamente', 'success');
        },
        error: (err) => {
          this.isEjecutando.set(false);
          if (!wasHandledByInterceptor(err)) {
            this.utilsService.showNotification('Error', 'Error al ejecutar el reporte', 'error');
          }
        },
      });
  }

  private loadReportes() {
    this.isLoadingReportes.set(true);
    this.reportesService.apiReportesReporteSearchGet().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.reporteOptions.set(res.data);
        }
        this.isLoadingReportes.set(false);
      },
      error: (err) => {
        this.isLoadingReportes.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar reportes', 'error');
        }
      },
    });
  }

  private loadConfiguracion(id: number) {
    this.isLoadingConfig.set(true);
    this.reportesService.getReporteConfiguracion(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.configuracion.set(res.data);
          this.parametros.set(res.data.parametros ?? []);
        } else {
          this.configuracion.set(null);
          this.parametros.set([]);
        }
        this.isLoadingConfig.set(false);
      },
      error: (err) => {
        this.isLoadingConfig.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar configuración del reporte', 'error');
        }
      },
    });
  }

  private triggerDownload(res: HttpResponse<Blob>) {
    const blob = res.body;
    if (!blob) return;

    const contentDisposition = res.headers.get('Content-Disposition') ?? '';
    let filename = 'reporte';
    const match = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
    if (match?.[1]) {
      filename = match[1].replace(/['"]/g, '');
    } else {
      const config = this.configuracion();
      const ext = config?.reporteFormatoId === 2 ? '.txt' : '.xlsx';
      filename = (config?.nomReporte ?? 'reporte').replace(/\s+/g, '_') + ext;
    }

    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
  }
}
