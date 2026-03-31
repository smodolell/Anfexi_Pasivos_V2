import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TipoPagoService } from '@services/catalogos/tipo-pago.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { TipoPagoListItemDto } from '../../../../types/catalogos/tipo-pago.dto';
import { UtilsService } from '@services/utils.service';
import { TipoPagoFormComponent } from './tipo-pago-form.component';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '@shared/components/generic-table/table-column.model';

@Component({
  selector: 'app-tipo-pago-list',
  standalone: true,
  imports: [CommonModule, TipoPagoFormComponent, ConfirmModalComponent, GenericTableComponent],
  templateUrl: './tipo-pago-list.component.html'
})
export class TipoPagoListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly tipoPagoService = inject(TipoPagoService);
  private readonly utilsService    = inject(UtilsService);

  items       = signal<TipoPagoListItemDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  mostrandoFormulario = signal(false);
  tipoPagoSeleccionado: Partial<TipoPagoListItemDto> = {};
  tipoPagoAEliminar: TipoPagoListItemDto | null = null;

  private q    = '';
  private page = 1;

  columns: TableColumn[] = [
    { key: 'id',       header: 'ID',           type: 'number', sortable: true },
    { key: 'tipoPago', header: 'Tipo de Pago', type: 'text',   sortable: true },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip',  variant: 'edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', variant: 'delete' },
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
    this.sortColumn   = event.column;
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
    this.tipoPagoSeleccionado = { id: 0, tipoPago: '' };
    this.mostrandoFormulario.set(true);
  }

  onAction(event: TableActionEvent<TipoPagoListItemDto>) {
    if (event.action === 'edit')   this.editar(event.row.id);
    if (event.action === 'delete') this.delete(event.row.id);
  }

  onGuardar(tipoPago: any) {
    const isUpdate = typeof tipoPago.id === 'number' && tipoPago.id > 0;
    const request$ = isUpdate
      ? this.tipoPagoService.update(tipoPago.id, tipoPago)
      : this.tipoPagoService.create(tipoPago);

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.load();
          this.mostrandoFormulario.set(false);
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al guardar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión', 'error'); }
    });
  }

  onCancelar() {
    this.volverALista();
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.tipoPagoSeleccionado = { id: 0, tipoPago: '' };
  }

  confirmarEliminacion() {
    if (!this.tipoPagoAEliminar) return;
    this.tipoPagoService.delete(this.tipoPagoAEliminar.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.load();
          this.confirmModal.hide();
          this.tipoPagoAEliminar = null;
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al eliminar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al eliminar', 'error'); }
    });
  }

  cancelarEliminacion() {
    this.tipoPagoAEliminar = null;
  }

  private editar(id: number) {
    this.tipoPagoService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.tipoPagoSeleccionado = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión', 'error'); }
    });
  }

  private delete(id: number) {
    const item = this.items().find(t => t.id === id);
    if (!item) return;
    this.tipoPagoAEliminar = item;
    this.confirmModal.show();
  }

  private load() {
    this.loading.set(true);
    this.tipoPagoService.getAll({
      q: this.q,
      page: this.page,
      size: this.pageSize(),
      sortBy:  this.sortColumn  ?? undefined,
      sortDir: this.sortDirection,
    }).subscribe({
      next: (response) => {
        if (response.success) {
          const size  = response.data.pageSize  ?? 10;
          const total = response.data.totalCount ?? 0;
          this.items.set(response.data.results ?? []);
          this.currentPage.set(response.data.currentPage ?? this.page);
          this.pageSize.set(size);
          this.totalCount.set(total);
          this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
        } else {
          this.resetPagination();
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar tipos de pago';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.resetPagination();
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar tipos de pago', 'error');
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
