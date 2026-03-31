import {
  Component,
  inject,
  signal,
  ViewChild,
  ChangeDetectionStrategy,
  DestroyRef,
  effect,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogosService } from '@api/services/catalogos.service';
import { ColoniaDto, CreateColoniaDto, UpdateColoniaDto } from '@api/models/models';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { ColoniaFormComponent } from './colonia-form.component';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import {
  TableColumn,
  TableAction,
  TableActionEvent,
  TableSortEvent,
} from '@shared/components/generic-table/table-column.model';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';
import { map } from 'rxjs';
import { TableDataSource } from 'src/app/core/datasource/table.datasource';

@Component({
  selector: 'app-colonia-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ColoniaFormComponent,
    GenericTableComponent,
    ConfirmModalComponent,
  ],
  templateUrl: './colonia-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ColoniaListComponent {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;
  private readonly destroyRef = inject(DestroyRef);
  private readonly catalogosService = inject(CatalogosService);
  private readonly utilsService = inject(UtilsService);
  readonly ds = new TableDataSource<ColoniaDto>(
  (q) =>
    this.catalogosService.getColoniasPaginados(q.q, q.page, q.size, q.sortBy, q.sortDir).pipe(
      map((res: any) => ({
        success: res.success as boolean,
        data:    res.data,
        errors:  res.errors as string[] | null ?? null,
      })),
    ),
  inject(DestroyRef),
);
  exportLoading = signal<boolean>(false);

  coloniaSeleccionada: Partial<ColoniaDto> = {};
  mostrandoFormulario = signal<boolean>(false);
  coloniaAEliminar: ColoniaDto | null = null;
  constructor() {
    effect(() => {
      const e = this.ds.error();
      if (e) this.utilsService.showNotification('Error', e, 'error');
    });
    this.ds.load();
  }
  // ── Configuración de la tabla genérica ───────────────────────
  columns: TableColumn[] = [
    { key: 'id', header: 'ID', type: 'number', sortable: true },
    { key: 'sColonia', header: 'Colonia', type: 'text', sortable: true },
    { key: 'estado', header: 'Estado', type: 'text', sortable: true },
    { key: 'municipio', header: 'Municipio', type: 'text', sortable: true },
    { key: 'codigoPostal', header: 'Código Postal', type: 'text', sortable: false },
  ];

  actions: TableAction[] = [
    { id: 'edit', label: 'Editar', icon: 'fa-solid fa-pen-clip', variant: 'edit' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', variant: 'delete' },
  ];

  onSearch(value: string) {
    this.ds.search(value);
  }

  onSort(event: TableSortEvent) {
    this.ds.sort(event.column, event.direction as 'asc' | 'desc');
  }

  nextPage() {
    this.ds.nextPage();
  }
  prevPage() {
    this.ds.prevPage();
  }

  onAction(event: TableActionEvent<ColoniaDto>) {
    if (event.action === 'edit') this.editarColonia(event.row.id!);
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
          this.ds.reload();
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
      },
    });
  }

  onCancelarEdicion() {
    this.volverALista();
  }

  onExportar() {
    this.exportLoading.set(true);
    this.catalogosService
      .exportColonias(this.ds.searchValue() || undefined)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (blob: any) => {
          const url = URL.createObjectURL(blob as Blob);
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
        },
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
          this.ds.reload();
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
      },
    });
  }

  cancelarEliminacion() {
    this.coloniaAEliminar = null;
  }

  // ── Private ──────────────────────────────────────────────────

  private delete(colonia: ColoniaDto) {
    this.coloniaAEliminar = colonia;
    this.confirmModal.show();
  }

  private editarColonia(id: number) {
    this.catalogosService.getColoniaById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.coloniaSeleccionada = { ...res.data } as Partial<ColoniaDto>;
          this.mostrandoFormulario.set(true);
        } else {
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? res.message ?? 'Error al cargar',
            'error',
          );
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification(
            'Error',
            'Error de conexión al cargar la colonia',
            'error',
          );
      },
    });
  }
}
