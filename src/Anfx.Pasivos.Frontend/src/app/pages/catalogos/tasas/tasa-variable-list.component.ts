import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatalogosService } from '@api/services/catalogos.service';
import { TasaVariableListItemDto } from '@api/models/tasaVariableListItemDto';
import { TasaVariableDetalleDto } from '@api/models/tasaVariableDetalleDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';
import { GenericTableComponent } from 'src/app/shared/components/generic-table/generic-table.component';
import { ConfirmModalComponent } from 'src/app/shared/components/confirm-modal/confirm-modal.component';
import { SortDirection, TableAction, TableActionEvent, TableColumn, TableSortEvent } from 'src/app/shared/components/generic-table/table-column.model';
import { TasaVariableFormComponent } from './tasa-variable-form.component';

@Component({
  selector: 'app-tasa-variable-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ConfirmModalComponent, TasaVariableFormComponent],
  templateUrl: './tasa-variable-list.component.html'
})
export class TasaVariableListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService     = inject(UtilsService);

  items       = signal<TasaVariableListItemDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  mostrandoFormulario = signal(false);
  tasaSeleccionada: TasaVariableDetalleDto | null = null;
  tasaAEliminar: TasaVariableListItemDto | null = null;

  private q              = '';
  private page           = 1;
  private sortDescending = false;

  columns: TableColumn[] = [
    { key: 'id',              header: 'ID',              type: 'number', sortable: true },
    { key: 'nombre',          header: 'Nombre',          type: 'text',   sortable: true },
    { key: 'ultimoValor',     header: 'Último Valor (%)', type: 'number', sortable: false },
    { key: 'fecUltimoValor',  header: 'Fecha Último Valor', type: 'date', sortable: false },
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
    this.sortColumn     = event.column;
    this.sortDirection  = event.direction;
    this.sortDescending = event.direction === 'desc';
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
    this.tasaSeleccionada = null;
    this.mostrandoFormulario.set(true);
  }

  onAction(event: TableActionEvent<TasaVariableListItemDto>) {
    if (event.action === 'edit')   this.editar(event.row.id!);
    if (event.action === 'delete') this.delete(event.row);
  }

  onGuardar(nombre: string) {
    const isUpdate = !!this.tasaSeleccionada?.id;
    const dto = { nombre };
    const request$ = isUpdate
      ? this.catalogosService.updateTasaVariable(this.tasaSeleccionada!.id!, dto)
      : this.catalogosService.createTasaVariable(dto);

    request$.subscribe({
      next: () => {
        const msg = isUpdate ? 'Tasa variable actualizada correctamente' : 'Tasa variable creada correctamente';
        this.utilsService.showNotification('Éxito', msg, 'success');
        this.load();
        this.mostrandoFormulario.set(false);
        this.tasaSeleccionada = null;
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al guardar la tasa variable', 'error');
      }
    });
  }

  onCancelar() {
    this.mostrandoFormulario.set(false);
    this.tasaSeleccionada = null;
  }

  onValoresChanged() {
    this.load();
  }

  confirmarEliminacion() {
    if (!this.tasaAEliminar) return;
    this.confirmModal.confirmLoading.set(true);
    this.catalogosService.deleteTasaVariable(this.tasaAEliminar.id!).subscribe({
      next: () => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.utilsService.showNotification('Éxito', 'Tasa variable eliminada correctamente', 'success');
        this.tasaAEliminar = null;
        this.load();
      },
      error: (err) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.tasaAEliminar = null;
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al eliminar la tasa variable', 'error');
      }
    });
  }

  cancelarEliminacion() {
    this.tasaAEliminar = null;
  }

  private editar(id: number) {
    this.catalogosService.getTasaVariableById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.tasaSeleccionada = res.data;
          this.mostrandoFormulario.set(true);
        } else {
          this.utilsService.showNotification('Error', 'No se pudo cargar la tasa variable', 'error');
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
      }
    });
  }

  private delete(item: TasaVariableListItemDto) {
    this.tasaAEliminar = item;
    this.confirmModal.show();
  }

  private load() {
    this.loading.set(true);
    this.catalogosService
      .apiCatalogosTasaVariableGet(
        this.q || undefined,
        this.page,
        this.pageSize(),
        this.sortColumn ?? undefined,
        this.sortDescending
      )
      .subscribe({
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
            this.utilsService.showNotification('Error', 'Error al cargar tasas variables', 'error');
          }
          this.loading.set(false);
        },
        error: (err) => {
          this.resetPagination();
          this.loading.set(false);
          if (!wasHandledByInterceptor(err))
            this.utilsService.showNotification('Error', 'Error de conexión al cargar tasas variables', 'error');
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
