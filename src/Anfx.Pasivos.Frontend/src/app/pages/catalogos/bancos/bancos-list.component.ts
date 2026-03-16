import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { BancoListItemDto } from '../../../../api/models/models';
import { UtilsService } from '../../../services/utils.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';

@Component({
  selector: 'app-bancos-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './bancos-list.component.html'
})
export class BancosListComponent implements OnInit {
  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService     = inject(UtilsService);
  private readonly router           = inject(Router);

  items       = signal<BancoListItemDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:     string | null = null;
  sortDirection:  SortDirection = 'asc';
  searchValue = '';

  private q              = '';
  private page           = 1;
  private sortDescending = false;

  // ── Columnas ─────────────────────────────────────────────────
  columns: TableColumn[] = [
    { key: 'id',    header: 'ID',              type: 'number', sortable: true },
    { key: 'banco', header: 'Nombre del Banco', type: 'text',  sortable: true },
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
    this.sortColumn     = event.column;
    this.sortDirection  = event.direction;
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
    this.router.navigate(['/catalogos/bancos/new']);
  }

  onAction(event: TableActionEvent<BancoListItemDto>) {
    if (event.action === 'edit') {
      this.router.navigate(['/catalogos/bancos/edit', event.row.id]);
    }
  }

  // ── Private ──────────────────────────────────────────────────
  private load() {
    this.loading.set(true);
    this.catalogosService
      .apiCatalogosBancoGet(
        this.q || undefined,
        this.page,
        this.pageSize(),
        this.sortColumn ?? undefined,
        this.sortDescending
      )
      .subscribe({
        next: (res) => {
          if (res.success && res.data) {
            const size  = res.data.pageSize ?? 10;
            const total = res.data.totalCount ?? 0;
            this.items.set(res.data.results ?? []);
            this.currentPage.set(res.data.currentPage ?? this.page);
            this.pageSize.set(size);
            this.totalCount.set(total);
            this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
          } else {
            this.resetPagination();
            this.utilsService.showNotification('Error', 'Error al cargar bancos', 'error');
          }
          this.loading.set(false);
        },
        error: (err) => {
          this.resetPagination();
          this.loading.set(false);
          if (!wasHandledByInterceptor(err)) {
            this.utilsService.showNotification('Error', 'Error de conexión al cargar bancos', 'error');
          }
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
