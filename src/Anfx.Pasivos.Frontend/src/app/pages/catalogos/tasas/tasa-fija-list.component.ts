import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatalogosService } from '@api/services/catalogos.service';
import { TasaFijaListItemDto } from '@api/models/tasaFijaListItemDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';
import { GenericTableComponent } from 'src/app/shared/components/generic-table/generic-table.component';
import { ConfirmModalComponent } from 'src/app/shared/components/confirm-modal/confirm-modal.component';
import { SortDirection, TableAction, TableActionEvent, TableColumn, TableSortEvent } from 'src/app/shared/components/generic-table/table-column.model';
import { TasaFijaFormComponent } from './tasa-fija-form.component';

@Component({
  selector: 'app-tasa-fija-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ConfirmModalComponent, TasaFijaFormComponent],
  templateUrl: './tasa-fija-list.component.html'
})
export class TasaFijaListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService     = inject(UtilsService);

  items       = signal<TasaFijaListItemDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  mostrandoFormulario = signal(false);
  tasaSeleccionada: Partial<TasaFijaListItemDto> = {};
  tasaAEliminar: TasaFijaListItemDto | null = null;

  private q              = '';
  private page           = 1;
  private sortDescending = false;

  columns: TableColumn[] = [
    { key: 'id',         header: 'ID',          type: 'number', sortable: true },
    { key: 'nombre',     header: 'Nombre',       type: 'text',   sortable: true },
    { key: 'valorTasa',  header: 'Valor (%)',    type: 'number', sortable: true },
    { key: 'fecTasa',    header: 'Fecha',        type: 'date',   sortable: true },
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
    this.tasaSeleccionada = {};
    this.mostrandoFormulario.set(true);
  }

  onAction(event: TableActionEvent<TasaFijaListItemDto>) {
    if (event.action === 'edit')   this.editar(event.row.id!);
    if (event.action === 'delete') this.delete(event.row);
  }

  onGuardar(tasa: any) {
    const isUpdate = typeof tasa.id === 'number' && tasa.id > 0;
    const dto = { nombre: tasa.nombre, valorTasa: tasa.valorTasa, fecTasa: tasa.fecTasa || null };
    const request$ = isUpdate
      ? this.catalogosService.updateTasaFija(tasa.id, dto)
      : this.catalogosService.createTasaFija(dto);

    request$.subscribe({
      next: () => {
        const msg = isUpdate ? 'Tasa fija actualizada correctamente' : 'Tasa fija creada correctamente';
        this.utilsService.showNotification('Éxito', msg, 'success');
        this.load();
        this.mostrandoFormulario.set(false);
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al guardar la tasa fija', 'error');
      }
    });
  }

  onCancelar() {
    this.mostrandoFormulario.set(false);
    this.tasaSeleccionada = {};
  }

  confirmarEliminacion() {
    if (!this.tasaAEliminar) return;
    this.confirmModal.confirmLoading.set(true);
    this.catalogosService.deleteTasaFija(this.tasaAEliminar.id!).subscribe({
      next: () => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.utilsService.showNotification('Éxito', 'Tasa fija eliminada correctamente', 'success');
        this.tasaAEliminar = null;
        this.load();
      },
      error: (err) => {
        this.confirmModal.confirmLoading.set(false);
        this.confirmModal.hide();
        this.tasaAEliminar = null;
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al eliminar la tasa fija', 'error');
      }
    });
  }

  cancelarEliminacion() {
    this.tasaAEliminar = null;
  }

  private editar(id: number) {
    this.catalogosService.getTasaFijaById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.tasaSeleccionada = { id, ...res.data };
          this.mostrandoFormulario.set(true);
        } else {
          this.utilsService.showNotification('Error', 'No se pudo cargar la tasa fija', 'error');
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
      }
    });
  }

  private delete(item: TasaFijaListItemDto) {
    this.tasaAEliminar = item;
    this.confirmModal.show();
  }

  private load() {
    this.loading.set(true);
    this.catalogosService
      .apiCatalogosTasaFijaGet(
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
            this.utilsService.showNotification('Error', 'Error al cargar tasas fijas', 'error');
          }
          this.loading.set(false);
        },
        error: (err) => {
          this.resetPagination();
          this.loading.set(false);
          if (!wasHandledByInterceptor(err))
            this.utilsService.showNotification('Error', 'Error de conexión al cargar tasas fijas', 'error');
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
