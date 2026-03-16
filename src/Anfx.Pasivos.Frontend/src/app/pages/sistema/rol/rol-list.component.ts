import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { RolService } from '../../../services/sistema/rol.service';
import { UtilsService } from '../../../services/utils.service';
import { RolFormComponent } from './rol-form.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '../../../shared/components/generic-table/table-column.model';
import { RolDto, RolPageQueryDto, CreateRolDto, UpdateRolDto } from '../../../../types/sistema/rol.dto';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-rol-list',
  standalone: true,
  imports: [RolFormComponent, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './rol-list.component.html',
})
export class RolListComponent implements OnInit {
  private readonly rolService   = inject(RolService);
  private readonly utilsService = inject(UtilsService);

  // ── Tabla ────────────────────────────────────────────────────
  columns: TableColumn[] = [
    { key: 'sRol',       header: 'Rol',         type: 'text', sortable: true, maxWidth: 180 },
    { key: 'descripcion',header: 'Descripción', type: 'text', sortable: true, maxWidth: 320, hideBelow: 'sm' },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-to-square', btnClass: 'btn-outline-primary' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can',     btnClass: 'btn-outline-danger'  },
  ];

  // ── Estado ───────────────────────────────────────────────────
  items       = signal<RolDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  query: RolPageQueryDto = { q: '', page: 1, size: 10 };

  // ── Formulario ───────────────────────────────────────────────
  mostrandoFormulario = signal(false);
  rolSeleccionado: Partial<RolDto> = {};

  // ── Modal eliminación ─────────────────────────────────────────
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;
  rolAEliminar: RolDto | null = null;

  ngOnInit(): void {
    this.load();
  }

  // ── Eventos del GenericTable ──────────────────────────────────

  onAction(event: TableActionEvent) {
    switch (event.action) {
      case 'edit':   this.editarRol((event.row as RolDto).id);     break;
      case 'delete': this.iniciarEliminacion(event.row as RolDto); break;
    }
  }

  onSearch(q: string) {
    this.query.q    = q;
    this.query.page = 1;
    this.load();
  }

  onPageNext() {
    if (this.currentPage() < this.totalPages()) {
      this.query.page = (this.query.page ?? 1) + 1;
      this.load();
    }
  }

  onPagePrev() {
    if ((this.query.page ?? 1) > 1) {
      this.query.page = (this.query.page ?? 1) - 1;
      this.load();
    }
  }

  onNuevo() {
    this.rolSeleccionado = { id: 0, sRol: '', descripcion: '' };
    this.mostrandoFormulario.set(true);
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.rolSeleccionado = {};
  }

  // ── CRUD ──────────────────────────────────────────────────────

  private load() {
    this.loading.set(true);
    this.rolService.getAll(this.query).subscribe({
      next: res => {
        if (res.success) {
          this.items.set(res.data.results);
          this.currentPage.set(res.data.currentPage);
          this.pageSize.set(res.data.pageSize);
          this.totalCount.set(res.data.totalCount);
          this.totalPages.set(res.data.totalPages);
        } else {
          this.items.set([]);
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al cargar roles', 'error');
        }
        this.loading.set(false);
      },
      error: () => {
        this.items.set([]);
        this.loading.set(false);
        this.utilsService.showNotification('Error', 'Error de conexión al cargar roles', 'error');
      },
    });
  }

  private editarRol(id: number) {
    this.rolService.getById(id).subscribe({
      next: res => {
        if (res.success) {
          this.rolSeleccionado = { ...res.data };
          this.mostrandoFormulario.set(true);
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al cargar el rol', 'error');
        }
      },
      error: () => this.utilsService.showNotification('Error', 'Error de conexión al cargar el rol', 'error'),
    });
  }

  onGuardarRol(rol: CreateRolDto | UpdateRolDto) {
    const id = this.rolSeleccionado.id;
    const isUpdate = id != null && id > 0;
    const request$ = isUpdate
      ? this.rolService.update(id, rol as UpdateRolDto)
      : this.rolService.create(rol as CreateRolDto);

    request$.subscribe({
      next: res => {
        if (res.success) {
          this.load();
          this.mostrandoFormulario.set(false);
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al guardar el rol', 'error');
        }
      },
      error: () => this.utilsService.showNotification('Error', 'Error de conexión al guardar el rol', 'error'),
    });
  }

  onCancelarEdicion() {
    this.volverALista();
  }

  // ── Eliminación ───────────────────────────────────────────────

  private iniciarEliminacion(rol: RolDto) {
    this.rolAEliminar = rol;
    this.confirmModal.show();
  }

  confirmarEliminacion() {
    if (!this.rolAEliminar) return;
    this.rolService.delete(this.rolAEliminar.id).subscribe({
      next: res => {
        if (res.success) {
          this.load();
          this.confirmModal.hide();
          this.rolAEliminar = null;
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al eliminar', 'error');
        }
      },
      error: () => this.utilsService.showNotification('Error', 'Error de conexión al eliminar el rol', 'error'),
    });
  }

  cancelarEliminacion() {
    this.rolAEliminar = null;
  }
}
