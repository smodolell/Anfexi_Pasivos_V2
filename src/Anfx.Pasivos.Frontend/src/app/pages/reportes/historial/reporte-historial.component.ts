import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { ReportesService } from '@api/services/reportes.service';
import { Configuration } from '@api/configuration';
import { ConfirmModalComponent } from 'src/app/shared/components/confirm-modal/confirm-modal.component';
import { ArchivoListItemDto } from '@api/models/archivoListItemDto';
import { SelectReporteDto } from '@api/models/selectReporteDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';

@Component({
  selector: 'app-reporte-historial',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmModalComponent],
  templateUrl: './reporte-historial.component.html',
})
export class ReporteHistorialComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly reportesService = inject(ReportesService);
  private readonly utilsService = inject(UtilsService);
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(Configuration);

  items = signal<ArchivoListItemDto[]>([]);
  reporteOptions = signal<SelectReporteDto[]>([]);
  archivoToDelete: ArchivoListItemDto | null = null;

  isLoading = signal(false);
  isLoadingReportes = signal(false);
  isDownloading = signal<string | null>(null);

  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);
  pageSize = signal(15);

  filtroReporteId: number | null = null;
  sortColumn = 'fechaCreacion';
  sortDescending = true;

  private page = 1;

  ngOnInit() {
    this.loadReportes();
    this.load();
  }

  onFiltroChange() {
    this.page = 1;
    this.load();
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.page++;
      this.load();
    }
  }

  prevPage() {
    if (this.page > 1) {
      this.page--;
      this.load();
    }
  }

  onDownload(archivo: ArchivoListItemDto) {
    if (!archivo.id) return;
    this.isDownloading.set(archivo.id);

    const url = `${this.apiConfig.basePath}/api/reportes/archivo/${archivo.id}/download`;
    this.http
      .get(url, { observe: 'response', responseType: 'blob' })
      .subscribe({
        next: (res: HttpResponse<Blob>) => {
          this.isDownloading.set(null);
          this.triggerDownload(res, archivo);
        },
        error: (err) => {
          this.isDownloading.set(null);
          if (!wasHandledByInterceptor(err)) {
            this.utilsService.showNotification('Error', 'Error al descargar el archivo', 'error');
          }
        },
      });
  }

  onDelete(archivo: ArchivoListItemDto) {
    this.archivoToDelete = archivo;
    this.confirmModal.show();
  }

  confirmDelete() {
    if (!this.archivoToDelete?.id) return;
    this.confirmModal.confirmLoading.set(true);

    this.reportesService.deleteArchivo(this.archivoToDelete.id).subscribe({
      next: (res: any) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.archivoToDelete = null;
        if (res?.success === false) {
          this.utilsService.showNotification('Error', res.message ?? 'No se pudo eliminar el archivo', 'error');
        } else {
          this.utilsService.showNotification('Éxito', 'Archivo eliminado correctamente', 'success');
          this.load();
        }
      },
      error: (err) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.archivoToDelete = null;
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar el archivo', 'error');
        }
      },
    });
  }

  private load() {
    this.isLoading.set(true);
    this.reportesService
      .apiReportesArchivoGet(
        this.filtroReporteId ?? undefined,
        this.page,
        this.pageSize(),
        this.sortColumn,
        this.sortDescending
      )
      .subscribe({
        next: (res) => {
          if (res.success && res.data) {
            const size = res.data.pageSize ?? 15;
            const total = res.data.totalCount ?? 0;
            this.items.set(res.data.results ?? []);
            this.currentPage.set(res.data.currentPage ?? this.page);
            this.pageSize.set(size);
            this.totalCount.set(total);
            this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
          } else {
            this.resetPagination();
          }
          this.isLoading.set(false);
        },
        error: (err) => {
          this.resetPagination();
          this.isLoading.set(false);
          if (!wasHandledByInterceptor(err)) {
            this.utilsService.showNotification('Error', 'Error al cargar el historial', 'error');
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
      error: () => this.isLoadingReportes.set(false),
    });
  }

  private triggerDownload(res: HttpResponse<Blob>, archivo: ArchivoListItemDto) {
    const blob = res.body;
    if (!blob) return;

    const contentDisposition = res.headers.get('Content-Disposition') ?? '';
    let filename = archivo.nombreArchivo ?? 'archivo';
    const match = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
    if (match?.[1]) {
      filename = match[1].replace(/['"]/g, '');
    }

    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
  }

  private resetPagination() {
    this.items.set([]);
    this.currentPage.set(this.page);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }
}
