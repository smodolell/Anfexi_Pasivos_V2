import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { UsuarioService } from '../../../services/sistema/usuario.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UsuarioItemDto, UsuarioDto, CreateUsuarioDto, UpdateUsuarioDto, UsuarioPageQueryDto } from '../../../../types/sistema/usuario.dto';
import { UtilsService } from '../../../services/utils.service';
import { UsuarioFormComponent } from './usuario-form.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '../../../shared/components/generic-table/table-column.model';
import { FilterActivoComponent } from '../../../shared/components/filter-activo/filter-activo.component';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-usuario-list',
  standalone: true,
  imports: [UsuarioFormComponent, GenericTableComponent, FilterActivoComponent, ConfirmModalComponent],
  templateUrl: './usuario-list.component.html',
})
export class UsuarioListComponent implements OnInit {
  private readonly usuarioService = inject(UsuarioService);
  private readonly utilsService  = inject(UtilsService);

  // ── Tabla ────────────────────────────────────────────────────
  columns: TableColumn[] = [
    { key: 'id', header: 'ID',      type: 'number', sortable: true, maxWidth: 80 },
    { key: 'nombreCompleto', header: 'Nombre',  type: 'text', sortable: true, maxWidth: 220 },
    { key: 'email',          header: 'Email',   type: 'text', sortable: true, maxWidth: 200, hideBelow: 'sm' },
    { key: 'usuarioNombre',  header: 'Usuario', type: 'text', sortable: true, hideBelow: 'md' },
    { key: 'rolNombre',      header: 'Rol',     type: 'text', hideBelow: 'lg' },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-user-pen',  btnClass: 'btn-outline-primary' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-user-minus', btnClass: 'btn-outline-danger'  },
  ];

  // ── Estado ───────────────────────────────────────────────────
  items       = signal<UsuarioItemDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  query: UsuarioPageQueryDto = { q: '', page: 1, size: 10, activo: true };

  // ── Formulario ───────────────────────────────────────────────
  mostrandoFormulario  = signal(false);
  usuarioSeleccionado: Partial<UsuarioDto> = {};

  // ── Modal eliminación ─────────────────────────────────────────
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;
  usuarioAEliminar: UsuarioItemDto | null = null;

  ngOnInit(): void {
    this.load();
  }

  // ── Eventos del GenericTable ──────────────────────────────────

  onAction(event: TableActionEvent) {
    switch (event.action) {
      case 'edit':   this.editarUsuario((event.row as UsuarioItemDto).id);  break;
      case 'delete': this.iniciarEliminacion(event.row as UsuarioItemDto);  break;
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

  onActivoChange() {
    this.query.page = 1;
    this.load();
  }

  onNuevo() {
    this.usuarioSeleccionado = { id: 0, nombreCompleto: '', email: '', usuarioNombre: '', activo: true, rolId: 0 };
    this.mostrandoFormulario.set(true);
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.usuarioSeleccionado = {};
  }

  // ── CRUD ──────────────────────────────────────────────────────

  private load() {
    this.loading.set(true);
    this.usuarioService.getAll(this.query).subscribe({
      next: res => {
        if (res.success) {
          this.items.set(res.data.results);
          this.currentPage.set(res.data.currentPage);
          this.pageSize.set(res.data.pageSize);
          this.totalCount.set(res.data.totalCount);
          this.totalPages.set(res.data.totalPages);
        } else {
          this.items.set([]);
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al cargar usuarios', 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.items.set([]);
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar usuarios', 'error');
        }
      },
    });
  }

  private editarUsuario(id: number) {
    this.usuarioService.getById(id).subscribe({
      next: res => {
        if (res.success) {
          this.usuarioSeleccionado = { ...res.data };
          this.mostrandoFormulario.set(true);
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al cargar el usuario', 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al cargar el usuario', 'error'); },
    });
  }

  onGuardarUsuario(usuario: CreateUsuarioDto | UpdateUsuarioDto) {
    const id = this.usuarioSeleccionado.id;
    const isUpdate = id != null && id > 0;
    const request$ = isUpdate
      ? this.usuarioService.update(id, usuario as UpdateUsuarioDto)
      : this.usuarioService.create(usuario as CreateUsuarioDto);

    request$.subscribe({
      next: res => {
        if (res.success) {
          this.load();
          this.mostrandoFormulario.set(false);
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al guardar el usuario', 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al guardar el usuario', 'error'); },
    });
  }

  onCancelarEdicion() {
    this.volverALista();
  }

  // ── Eliminación ───────────────────────────────────────────────

  private iniciarEliminacion(usuario: UsuarioItemDto) {
    this.usuarioAEliminar = usuario;
    this.confirmModal.show();
  }

  confirmarEliminacion() {
    if (!this.usuarioAEliminar) return;
    this.usuarioService.delete(this.usuarioAEliminar.id).subscribe({
      next: res => {
        if (res.success) {
          this.load();
          this.confirmModal.hide();
          this.usuarioAEliminar = null;
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al eliminar', 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al eliminar el usuario', 'error'); },
    });
  }

  cancelarEliminacion() {
    this.usuarioAEliminar = null;
  }
}
