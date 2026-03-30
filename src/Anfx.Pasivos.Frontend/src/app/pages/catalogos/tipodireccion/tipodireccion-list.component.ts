import { Component, inject, signal, ViewChild, DestroyRef, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TipoDireccionService } from '../../../services/catalogos/tipodireccion.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { TipoDireccionDto } from '../../../../types/catalogos/tipodireccion.dto';
import { UtilsService } from '../../../services/utils.service';
import { TipoDireccionFormComponent } from './tipodireccion-form.component';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import {
  TableColumn,
  TableAction,
  TableActionEvent,
  TableSortEvent,
} from '../../../shared/components/generic-table/table-column.model';
import { TableDataSource } from '../../../core/datasource/table.datasource';

@Component({
  selector: 'app-tipodireccion-list',
  standalone: true,
  imports: [CommonModule, TipoDireccionFormComponent, ConfirmModalComponent, GenericTableComponent],
  templateUrl: './tipodireccion-list.component.html',
})
export class TipoDireccionListComponent {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly service = inject(TipoDireccionService);
  private readonly utilsService = inject(UtilsService);

  // ── DataSource (reemplaza: items, loading, totalCount, totalPages,
  //   currentPage, pageSize, sortColumn, sortDirection, searchValue,
  //   onSearch, onSort, nextPage, prevPage, load, resetPagination) ──
  readonly ds = new TableDataSource<TipoDireccionDto>(
    (q) => this.service.getAll(q),
    inject(DestroyRef),
  );

  // ── Estado CRUD (no cambia) ────────────────────────────────────
  mostrandoFormulario = signal(false);
  tipoDireccionSeleccionado: Partial<TipoDireccionDto> = {};
  tipoDireccionAEliminar: TipoDireccionDto | null = null;

  columns: TableColumn[] = [
    { key: 'id', header: 'ID', type: 'number', sortable: true },
    { key: 'sTipoDireccion', header: 'Título', type: 'text', sortable: true },
  ];

  actions: TableAction[] = [
    { id: 'edit', label: 'Editar', icon: 'fa-solid fa-pen-clip', variant: 'edit' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', variant: 'delete' },
  ];

  constructor() {
    // Centraliza notificaciones de error del datasource
    effect(() => {
      const err = this.ds.error();
      if (err) this.utilsService.showNotification('Error', err, 'error');
    });
    this.ds.load();
  }

  // ── Delegación al DataSource ───────────────────────────────────
  onSearch(value: string) {
    this.ds.search(value);
  }
  onSort(e: TableSortEvent) {
    this.ds.sort(e.column, e.direction as 'asc' | 'desc');
  }
  nextPage() {
    this.ds.nextPage();
  }
  prevPage() {
    this.ds.prevPage();
  }

  // ── CRUD (sin cambios en lógica) ───────────────────────────────
  onNuevo() {
    this.tipoDireccionSeleccionado = { id: 0, sTipoDireccion: '' };
    this.mostrandoFormulario.set(true);
  }

  onAction(event: TableActionEvent<TipoDireccionDto>) {
    if (event.action === 'edit') this.editarTipoDireccion(event.row.id);
    if (event.action === 'delete') this.iniciarEliminacion(event.row);
  }

  onExportar() {
    this.service.exportar(this.ds.searchValue()).subscribe({
      next: (blob) => this.descargarBlob(blob, 'tipos_direcciones'),
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al exportar', 'error');
      },
    });
  }

  onGuardarTipoDireccion(tipoDireccion: any) {
    const isUpdate = typeof tipoDireccion.id === 'number' && tipoDireccion.id > 0;
    const req$ = isUpdate
      ? this.service.update(tipoDireccion.id, tipoDireccion)
      : this.service.create(tipoDireccion);

    req$.subscribe({
      next: (res) => {
        if (res.success) {
          this.ds.reload();
          this.mostrandoFormulario.set(false);
        } else
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? 'Error al guardar',
            'error',
          );
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
      },
    });
  }

  onCancelarEdicion() {
    this.mostrandoFormulario.set(false);
  }

  confirmarEliminacion() {
    if (!this.tipoDireccionAEliminar) return;
    this.service.delete(this.tipoDireccionAEliminar.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.ds.reload();
          this.confirmModal.hide();
          this.tipoDireccionAEliminar = null;
        } else
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? 'Error al eliminar',
            'error',
          );
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error al eliminar', 'error');
      },
    });
  }

  cancelarEliminacion() {
    this.tipoDireccionAEliminar = null;
  }

  private editarTipoDireccion(id: number) {
    this.service.getById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.tipoDireccionSeleccionado = { ...res.data };
          this.mostrandoFormulario.set(true);
        } else
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? 'Error al cargar',
            'error',
          );
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification('Error', 'Error de conexión', 'error');
      },
    });
  }

  private iniciarEliminacion(item: TipoDireccionDto) {
    this.tipoDireccionAEliminar = item;
    this.confirmModal.show();
  }

  private descargarBlob(blob: Blob, nombre: string) {
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    const fecha = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
    link.href = url;
    link.download = `${nombre}_${fecha}.xlsx`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
    this.utilsService.showNotification('Éxito', 'Archivo exportado correctamente', 'success');
  }
  volverAlListado(){
    this.mostrandoFormulario.set(false);
  }
}
