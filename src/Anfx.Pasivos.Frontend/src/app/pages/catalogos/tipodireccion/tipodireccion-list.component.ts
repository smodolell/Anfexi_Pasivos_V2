import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TipoDireccionService } from '../../../services/catalogos/tipodireccion.service';
import { TipoDireccionDto } from '../../../../types/catalogos/tipodireccion.dto';
import { UtilsService } from '../../../services/utils.service';
import { TipoDireccionFormComponent } from './tipodireccion-form.component';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';

@Component({
  selector: 'app-tipodireccion-list',
  standalone: true,
  imports: [CommonModule, TipoDireccionFormComponent, ConfirmModalComponent, GenericTableComponent],
  templateUrl: './tipodireccion-list.component.html'
})
export class TipoDireccionListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly tipoDireccionService = inject(TipoDireccionService);
  private readonly utilsService         = inject(UtilsService);

  items       = signal<TipoDireccionDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  mostrandoFormulario    = signal(false);
  tipoDireccionSeleccionado: Partial<TipoDireccionDto> = {};
  tipoDireccionAEliminar: TipoDireccionDto | null = null;

  private q    = '';
  private page = 1;

  columns: TableColumn[] = [
    { key: 'id',             header: 'ID',     type: 'number', sortable: true },
    { key: 'sTipoDireccion', header: 'Título', type: 'text',   sortable: true },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip',  btnClass: 'btn-action-edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', btnClass: 'btn-action-delete' },
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
    this.tipoDireccionSeleccionado = { id: 0, sTipoDireccion: '' };
    this.mostrandoFormulario.set(true);
  }

  onAction(event: TableActionEvent<TipoDireccionDto>) {
    if (event.action === 'edit')   this.editarTipoDireccion(event.row.id);
    if (event.action === 'delete') this.delete(event.row.id);
  }

  onExportar() {
    this.loading.set(true);
    this.tipoDireccionService.exportar(this.q).subscribe({
      next: (blob) => {
        const url  = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href  = url;
        const fecha = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
        link.download = `tipos_direcciones_${fecha}.xlsx`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
        this.loading.set(false);
        this.utilsService.showNotification('Éxito', 'Archivo exportado correctamente', 'success');
      },
      error: () => {
        this.loading.set(false);
        this.utilsService.showNotification('Error', 'Error al exportar el archivo', 'error');
      }
    });
  }

  onGuardarTipoDireccion(tipoDireccion: any) {
    const isUpdate = 'Id' in tipoDireccion && tipoDireccion.Id && typeof tipoDireccion.Id === 'number';
    const request$ = isUpdate
      ? this.tipoDireccionService.update(tipoDireccion.Id, tipoDireccion)
      : this.tipoDireccionService.create(tipoDireccion);

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
      error: () => this.utilsService.showNotification('Error', 'Error de conexión', 'error')
    });
  }

  onCancelarEdicion() {
    this.volverALista();
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.tipoDireccionSeleccionado = { id: 0, sTipoDireccion: '' };
  }

  confirmarEliminacion() {
    if (!this.tipoDireccionAEliminar) return;
    this.tipoDireccionService.delete(this.tipoDireccionAEliminar.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.load();
          this.confirmModal.hide();
          this.tipoDireccionAEliminar = null;
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al eliminar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: () => this.utilsService.showNotification('Error', 'Error de conexión al eliminar', 'error')
    });
  }

  cancelarEliminacion() {
    this.tipoDireccionAEliminar = null;
  }

  private editarTipoDireccion(id: number) {
    this.tipoDireccionService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.tipoDireccionSeleccionado = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: () => this.utilsService.showNotification('Error', 'Error de conexión', 'error')
    });
  }

  private delete(id: number) {
    const item = this.items().find(t => t.id === id);
    if (!item) return;
    this.tipoDireccionAEliminar = item;
    this.confirmModal.show();
  }

  private load() {
    this.loading.set(true);
    this.tipoDireccionService.getAll({ q: this.q, page: this.page, size: this.pageSize() }).subscribe({
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
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar tipos de dirección';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: () => {
        this.resetPagination();
        this.loading.set(false);
        this.utilsService.showNotification('Error', 'Error de conexión al cargar tipos de dirección', 'error');
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
