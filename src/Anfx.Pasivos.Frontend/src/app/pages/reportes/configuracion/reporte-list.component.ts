import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReportesService } from '@api/services/reportes.service';
import { ReporteListItemDto } from '@api/models/reporteListItemDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';
import { GenericTableComponent } from 'src/app/shared/components/generic-table/generic-table.component';
import { ConfirmModalComponent } from 'src/app/shared/components/confirm-modal/confirm-modal.component';
import { SortDirection, TableAction, TableActionEvent, TableColumn, TableSortEvent } from 'src/app/shared/components/generic-table/table-column.model';

@Component({
  selector: 'app-reporte-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './reporte-list.component.html',
})
export class ReporteListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly reportesService = inject(ReportesService);
  private readonly utilsService = inject(UtilsService);
  private readonly router = inject(Router);

  reporteToDelete: ReporteListItemDto | null = null;

  items = signal<ReporteListItemDto[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);

  sortColumn: string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  private q = '';
  private page = 1;
  private sortDescending = false;

  columns: TableColumn[] = [
    { key: 'id',              header: 'ID',               type: 'number',  sortable: true },
    { key: 'nomReporte',      header: 'Nombre',           type: 'text',    sortable: true },
    { key: 'storedProcedure', header: 'Stored Procedure', type: 'text',    sortable: false },
    { key: 'parametros',      header: 'Parámetros',       type: 'text',    sortable: false },
    { key: 'activo',          header: 'Activo',           type: 'boolean', sortable: false },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip', btnClass: 'btn-action-edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash',    btnClass: 'btn-action-delete' },
  ];

  ngOnInit() {
    this.load();
  }

  onSearch(value: string) {
    this.q = value;
    this.searchValue = value;
    this.page = 1;
    this.load();
  }

  onSort(event: TableSortEvent) {
    this.sortColumn = event.column;
    this.sortDirection = event.direction;
    this.sortDescending = event.direction === 'desc';
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

  onNuevo() {
    this.router.navigate(['/reportes/configuracion/new']);
  }

  onAction(event: TableActionEvent<ReporteListItemDto>) {
    if (event.action === 'edit') {
      this.router.navigate(['/reportes/configuracion/edit', event.row.id]);
    }
    if (event.action === 'delete') {
      this.reporteToDelete = event.row;
      this.confirmModal.show();
    }
  }

  confirmDelete() {
    if (!this.reporteToDelete) return;
    this.confirmModal.confirmLoading.set(true);

    this.reportesService.deleteReporte(this.reporteToDelete.id!).subscribe({
      next: (res: any) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.reporteToDelete = null;
        if (res?.success === false) {
          this.utilsService.showNotification('Error', res.message ?? 'No se pudo eliminar el reporte', 'error');
        } else {
          this.utilsService.showNotification('Éxito', 'Reporte eliminado correctamente', 'success');
          this.load();
        }
      },
      error: (err) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.reporteToDelete = null;
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar el reporte', 'error');
        }
      },
    });
  }

  private load() {
    this.loading.set(true);
    this.reportesService
      .apiReportesReporteGet(
        this.q || undefined,
        this.page,
        this.pageSize(),
        this.sortColumn ?? undefined,
        this.sortDescending
      )
      .subscribe({
        next: (res) => {
          if (res.success && res.data) {
            const size = res.data.pageSize ?? 10;
            const total = res.data.totalCount ?? 0;
            this.items.set(res.data.results ?? []);
            this.currentPage.set(res.data.currentPage ?? this.page);
            this.pageSize.set(size);
            this.totalCount.set(total);
            this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
          } else {
            this.resetPagination();
            this.utilsService.showNotification('Error', 'Error al cargar reportes', 'error');
          }
          this.loading.set(false);
        },
        error: (err) => {
          this.resetPagination();
          this.loading.set(false);
          if (!wasHandledByInterceptor(err)) {
            this.utilsService.showNotification('Error', 'Error de conexión al cargar reportes', 'error');
          }
        },
      });
  }

  private resetPagination() {
    this.items.set([]);
    this.currentPage.set(this.page);
    this.pageSize.set(10);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }
}
