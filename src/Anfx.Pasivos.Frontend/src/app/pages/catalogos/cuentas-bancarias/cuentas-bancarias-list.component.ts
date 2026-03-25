import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { CuentaBancariaListItemDto } from '../../../../api/models/models';
import { UtilsService } from '../../../services/utils.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-cuentas-bancarias-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './cuentas-bancarias-list.component.html'
})
export class CuentasBancariasListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService     = inject(UtilsService);
  private readonly router           = inject(Router);

  cuentaToDelete: CuentaBancariaListItemDto | null = null;

  items       = signal<CuentaBancariaListItemDto[]>([]);
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
    { key: 'banco',         header: 'Banco',          type: 'text', sortable: true  },
    { key: 'cuentaBancaria',header: 'Cuenta Bancaria', type: 'text', sortable: true  },
    { key: 'clabe',         header: 'CLABE',           type: 'text', sortable: false },
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
    this.router.navigate(['/catalogos/cuentas-bancarias/new']);
  }

  onAction(event: TableActionEvent<CuentaBancariaListItemDto>) {
    if (event.action === 'edit') {
      this.router.navigate(['/catalogos/cuentas-bancarias/edit', event.row.id]);
    }
    if (event.action === 'delete') {
      this.cuentaToDelete = event.row;
      this.confirmModal.show();
    }
  }

  confirmDelete() {
    if (!this.cuentaToDelete) return;
    this.confirmModal.confirmLoading.set(true);

    this.catalogosService.deleteCuentaBancaria(this.cuentaToDelete.id!).subscribe({
      next: (res: any) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.cuentaToDelete = null;
        if (res?.success === false) {
          this.utilsService.showNotification('Error', res.message ?? 'No se pudo eliminar la cuenta bancaria', 'error');
        } else {
          this.utilsService.showNotification('Éxito', 'Cuenta bancaria eliminada correctamente', 'success');
          this.load();
        }
      },
      error: (err) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.cuentaToDelete = null;
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar la cuenta bancaria', 'error');
        }
      }
    });
  }

  private load() {
    this.loading.set(true);
    this.catalogosService.apiCatalogosCuentaBancariaGet(this.q || undefined, this.page, this.pageSize()).subscribe({
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
          this.utilsService.showNotification('Error', 'Error al cargar cuentas bancarias', 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.resetPagination();
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar cuentas bancarias', 'error');
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
