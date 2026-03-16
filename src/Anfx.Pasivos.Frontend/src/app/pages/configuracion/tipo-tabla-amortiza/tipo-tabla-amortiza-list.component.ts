import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ConfiguracionesService } from '../../../../api/services/configuraciones.service';
import { TipoTablaAmortizaListItemDto } from '../../../../api/models/tipoTablaAmortizaListItemDto';
import { UtilsService } from '../../../services/utils.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';

@Component({
  selector: 'app-tipo-tabla-amortiza-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './tipo-tabla-amortiza-list.component.html'
})
export class TipoTablaAmortizaListComponent implements OnInit {
  private readonly service      = inject(ConfiguracionesService);
  private readonly utilsService = inject(UtilsService);
  private readonly router       = inject(Router);

  items       = signal<TipoTablaAmortizaListItemDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  private q    = '';
  private page = 1;

  columns: TableColumn[] = [
    { key: 'id',               header: 'ID',                       type: 'number',  sortable: true  },
    { key: 'tipoTablaAmortiza',header: 'Tipo Tabla Amortización',   type: 'text',    sortable: true  },
    { key: 'activo',           header: 'Activo',                    type: 'boolean', sortable: false },
  ];

  actions: TableAction[] = [
    { id: 'edit', label: 'Editar', icon: 'fa-solid fa-pen-clip', btnClass: 'btn-action-edit' },
  ];

  ngOnInit() {
    this.load();
  }

  onSearch(value: string) {
    this.q           = value;
    this.searchValue = value;
    this.page        = 1;
    this.load();
  }

  onSort(event: TableSortEvent) {
    this.sortColumn    = event.column;
    this.sortDirection = event.direction;
    this.page = 1;
    this.load();
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) { this.page++; this.load(); }
  }

  prevPage() {
    if (this.page > 1) { this.page--; this.load(); }
  }

  onNuevo() {
    this.router.navigate(['/configuracion/tipo-tabla-amortiza/new']);
  }

  onAction(event: TableActionEvent<TipoTablaAmortizaListItemDto>) {
    if (event.action === 'edit') {
      this.router.navigate(['/configuracion/tipo-tabla-amortiza/edit', event.row.id]);
    }
  }

  private load() {
    this.loading.set(true);
    this.service.apiConfiguracionesTipoTablaAmortizaGet(this.q || undefined, this.page, this.pageSize()).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const size  = res.data.pageSize  ?? 10;
          const total = res.data.totalCount ?? 0;
          this.items.set(res.data.results ?? []);
          this.currentPage.set(res.data.currentPage ?? this.page);
          this.pageSize.set(size);
          this.totalCount.set(total);
          this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
        } else {
          this.resetPagination();
          this.utilsService.showNotification('Error', 'Error al cargar tipos de tabla', 'error');
        }
        this.loading.set(false);
      },
      error: () => {
        this.resetPagination();
        this.loading.set(false);
        this.utilsService.showNotification('Error', 'Error de conexión al cargar tipos de tabla', 'error');
      }
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
