import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { BancoListItemDto } from '../../../../api/models/models';
import { UtilsService } from '../../../services/utils.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-bancos-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './bancos-list.component.html'
})
export class BancosListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService     = inject(UtilsService);
  private readonly router           = inject(Router);

  bancoToDelete: BancoListItemDto | null = null;

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
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip', btnClass: 'btn-action-edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash',    btnClass: 'btn-action-delete' },
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
    if (event.action === 'delete') {
      this.delete(event.row);
    }
  }

  private delete(item: BancoListItemDto) {
    this.bancoToDelete = item;
    this.confirmModal.show();
  }

  confirmDelete() {
    if (!this.bancoToDelete) return;
    this.confirmModal.confirmLoading = true;

    this.catalogosService.deleteBanco(this.bancoToDelete.id!).subscribe({
      next: (res: any) => {
        this.confirmModal.confirmLoading = false;
        this.confirmModal.hide();
        this.bancoToDelete = null;
        if (res?.success === false) {
          this.utilsService.showNotification('Error', res.message ?? 'No se pudo eliminar el banco', 'error');
        } else {
          this.utilsService.showNotification('Éxito', 'Banco eliminado correctamente', 'success');
          this.load();
        }
      },
      error: (err) => {
        this.confirmModal.confirmLoading = false;
        this.confirmModal.hide();
        this.bancoToDelete = null;
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar el banco', 'error');
        }
      }
    });
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
