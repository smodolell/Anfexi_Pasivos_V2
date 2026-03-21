import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ColoniaService } from '../../../services/catalogos/colonia.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { ColoniaDto, ColoniaPageQueryDto } from '../../../../types/catalogos/colonia.dto';
import { UtilsService } from '../../../services/utils.service';
import { ColoniaFormComponent } from './colonia-form.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';
import { CardComponent } from '../../../shared/components/card/card.component';

@Component({
  selector: 'app-colonia-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ColoniaFormComponent, GenericTableComponent, ConfirmModalComponent, CardComponent],
  templateUrl: './colonia-list.component.html'
})
export class ColoniaListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private coloniaService = inject(ColoniaService);
  private utilsService = inject(UtilsService);

  items         = signal<ColoniaDto[]>([]);
  loading       = signal<boolean>(false);
  exportLoading = signal<boolean>(false);
  query: ColoniaPageQueryDto = { q: '', page: 1, size: 10 };

  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);

  sortColumn: string | null = null;
  sortDirection: SortDirection = 'asc';

  coloniaSeleccionada: Partial<ColoniaDto> = {};
  mostrandoFormulario = signal<boolean>(false);
  coloniaAEliminar: ColoniaDto | null = null;

  // ── Configuración de la tabla genérica ───────────────────────
  columns: TableColumn[] = [
    { key: 'id',          header: 'ID',            type: 'number', sortable: true  },
    { key: 'sColonia',    header: 'Colonia',        type: 'text',   sortable: true  },
    { key: 'estado',      header: 'Estado',         type: 'text',   sortable: true  },
    { key: 'municipio',   header: 'Municipio',      type: 'text',   sortable: true  },
    { key: 'codigoPostal',header: 'Código Postal',  type: 'text',   sortable: false },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip',  variant: 'edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', variant: 'delete' },
  ];

  ngOnInit(): void {
    this.load();
  }

  onSearch(value: string) {
    this.query.q = value;
    this.query.page = 1;
    this.load();
  }

  onSort(event: TableSortEvent) {
    this.sortColumn = event.column;
    this.sortDirection = event.direction;
    this.query.sortBy = event.column;
    this.query.sortDir = event.direction;
    this.query.page = 1;
    this.load();
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.query.page = (this.query.page || 1) + 1;
      this.load();
    }
  }

  prevPage() {
    if ((this.query.page || 1) > 1) {
      this.query.page = (this.query.page || 1) - 1;
      this.load();
    }
  }

  onAction(event: TableActionEvent<ColoniaDto>) {
    if (event.action === 'edit')   this.editarColonia(event.row.id);
    if (event.action === 'delete') this.delete(event.row.id);
  }

  mostrarFormularioNuevo() {
    this.coloniaSeleccionada = { id: 0, sColonia: '', estado: '', municipio: '', codigoPostal: '' };
    this.mostrandoFormulario.set(true);
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.coloniaSeleccionada = { id: 0, sColonia: '', estado: '', municipio: '', codigoPostal: '' };
  }

  onGuardarColonia(colonia: any) {
    const isUpdate = typeof colonia.id === 'number' && colonia.id > 0;
    const request$ = isUpdate
      ? this.coloniaService.update(colonia.id, colonia)
      : this.coloniaService.create(colonia);

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

  onCancelarEdicion() { this.volverALista(); }

  onExportar() {
    this.exportLoading.set(true);
    this.coloniaService.exportar(this.query).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        const fecha = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
        link.download = `colonias_${fecha}.xlsx`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
        this.exportLoading.set(false);
        this.utilsService.showNotification('Éxito', 'Archivo exportado correctamente', 'success');
      },
      error: (err) => {
        this.exportLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al exportar', 'error');
        }
      }
    });
  }

  // ── Private ──────────────────────────────────────────────────
  private load() {
    this.loading.set(true);
    this.coloniaService.getAll(this.query).subscribe({
      next: (response) => {
        if (response.success) {
          this.items.set(response.data.results);
          this.currentPage.set(response.data.currentPage);
          this.pageSize.set(response.data.pageSize);
          this.totalCount.set(response.data.totalCount);
          this.totalPages.set(response.data.totalPages);
        } else {
          this.resetPagination();
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar colonias';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.resetPagination();
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar colonias', 'error');
        }
        console.error(err);
      }
    });
  }

  private editarColonia(id: number) {
    this.coloniaService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.coloniaSeleccionada = { ...response.data };
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
    const colonia = this.items().find(c => c.id === id);
    if (!colonia) return;
    this.coloniaAEliminar = colonia;
    this.confirmModal.show();
  }

  confirmarEliminacion() {
    if (!this.coloniaAEliminar) return;
    this.coloniaService.delete(this.coloniaAEliminar.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.load();
          this.confirmModal.hide();
          this.coloniaAEliminar = null;
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al eliminar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión', 'error'); }
    });
  }

  cancelarEliminacion() {
    this.coloniaAEliminar = null;
  }

  private resetPagination() {
    this.items.set([]);
    this.currentPage.set(this.query.page || 1);
    this.pageSize.set(this.query.size || 10);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }
}
