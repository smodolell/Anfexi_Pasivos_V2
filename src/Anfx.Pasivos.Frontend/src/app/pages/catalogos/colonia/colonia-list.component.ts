import { Component, OnInit, inject, signal, ViewChild, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { ColoniaDto, CreateColoniaDto, UpdateColoniaDto } from '../../../../api/models/models';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';
import { ColoniaFormComponent } from './colonia-form.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-colonia-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ColoniaFormComponent, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './colonia-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ColoniaListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;
  private readonly destroyRef = inject(DestroyRef);
  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService     = inject(UtilsService);

  items         = signal<ColoniaDto[]>([]);
  loading       = signal<boolean>(false);
  exportLoading = signal<boolean>(false);
  query = { q: '', page: 1, size: 10, sortBy: undefined as string | undefined, sortDir: undefined as string | undefined };

  totalCount  = signal<number>(0);
  totalPages  = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize    = signal<number>(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';

  coloniaSeleccionada: Partial<ColoniaDto> = {};
  searchValue         = signal<string>('');
  mostrandoFormulario = signal<boolean>(false);
  coloniaAEliminar: ColoniaDto | null = null;

  // ── Configuración de la tabla genérica ───────────────────────
  columns: TableColumn[] = [
    { key: 'id',          header: 'ID',           type: 'number', sortable: true  },
    { key: 'sColonia',    header: 'Colonia',       type: 'text',   sortable: true  },
    { key: 'estado',      header: 'Estado',        type: 'text',   sortable: true  },
    { key: 'municipio',   header: 'Municipio',     type: 'text',   sortable: true  },
    { key: 'codigoPostal',header: 'Código Postal', type: 'text',   sortable: false },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip',  variant: 'edit' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', variant: 'delete' },
  ];

  ngOnInit(): void {
    this.load();
  }

  onSearch(value: string) {
    this.searchValue.set(value);
    this.query.q    = value;
    this.query.page = 1;
    this.load();
  }

  onSort(event: TableSortEvent) {
    this.sortColumn    = event.column;
    this.sortDirection = event.direction;
    this.query.sortBy  = event.column;
    this.query.sortDir = event.direction;
    this.query.page    = 1;
    this.load();
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.query.page++;
      this.load();
    }
  }

  prevPage() {
    if (this.query.page > 1) {
      this.query.page--;
      this.load();
    }
  }

  onAction(event: TableActionEvent<ColoniaDto>) {
    if (event.action === 'edit')   this.editarColonia(event.row.id!);
    if (event.action === 'delete') this.delete(event.row);
  }

  mostrarFormularioNuevo() {
    this.coloniaSeleccionada = { sColonia: '', estado: '', municipio: '', codigoPostal: '' };
    this.mostrandoFormulario.set(true);
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.coloniaSeleccionada = {};
  }

  onGuardarColonia(colonia: any) {
    const isUpdate = typeof colonia.id === 'number' && colonia.id > 0;
    const request$ = isUpdate
      ? this.catalogosService.updateColonia(colonia.id, colonia as UpdateColoniaDto)
      : this.catalogosService.createColonia(colonia as CreateColoniaDto);

    request$.subscribe({
      next: (res) => {
        if (res.success) {
          this.load();
          this.mostrandoFormulario.set(false);
        } else {
          const msg = res.errors?.[0] ?? res.message ?? 'Error al guardar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
        }
      }
    });
  }

  onCancelarEdicion() { this.volverALista(); }

  onExportar() {
    this.exportLoading.set(true);
    this.catalogosService.exportColonias(this.query.q || undefined)
    .pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (blob: any) => {
        const url  = URL.createObjectURL(blob as Blob);
        const link = document.createElement('a');
        link.href  = url;
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

  confirmarEliminacion() {
    if (!this.coloniaAEliminar) return;
    this.confirmModal.confirmLoading.set(true);

    this.catalogosService.deleteColonia(this.coloniaAEliminar.id!).subscribe({
      next: (res) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.coloniaAEliminar = null;
        if (res.success) {
          this.utilsService.showNotification('Éxito', 'Colonia eliminada correctamente', 'success');
          this.load();
        } else {
          const msg = res.errors?.[0] ?? res.message ?? 'Error al eliminar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.coloniaAEliminar = null;
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
        }
      }
    });
  }

  cancelarEliminacion() {
    this.coloniaAEliminar = null;
  }

  // ── Private ──────────────────────────────────────────────────
  private load() {
    this.loading.set(true);
    this.catalogosService
      .getColoniasPaginados(
        this.query.q || undefined,
        this.query.page,
        this.query.size,
        this.query.sortBy,
        this.query.sortDir
      )
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res) => {

          if (res.success && res.data) {
            this.items.set(res.data.results ?? []);
            this.currentPage.set(res.data.currentPage ?? this.query.page);
            this.pageSize.set(res.data.pageSize ?? this.query.size);
            this.totalCount.set(res.data.totalCount ?? 0);
            this.totalPages.set(res.data.totalPages ?? 0);
          } else {
            this.resetPagination();
            const msg = res.errors?.[0] ?? 'Error al cargar colonias';
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
        }
      });
  }

  private editarColonia(id: number) {
    this.catalogosService.getColoniaById(id)
    .pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.coloniaSeleccionada = { ...res.data };
          this.mostrandoFormulario.set(true);
        } else {
          const msg = res.errors?.[0] ?? res.message ?? 'Error al cargar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
        }
      }
    });
  }

  private delete(colonia: ColoniaDto) {
    this.coloniaAEliminar = colonia;
    this.confirmModal.show();
  }

  private resetPagination() {
    this.items.set([]);
    this.currentPage.set(this.query.page);
    this.pageSize.set(this.query.size);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }
}
