import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { AuthService as AuthApiService } from '../../../../api/services/auth.service';
import { UsuarioCreateDto, UsuarioUpdateDto } from '../../../../api/models/models';
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
  private readonly authApiService = inject(AuthApiService);
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
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-user-pen',  variant: 'edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-user-minus', variant: 'delete' },
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
    this.authApiService.getUsuariosPaginados(
      this.query.q,
      this.query.page,
      this.query.size,
      this.query.activo,
    ).subscribe({
      next: res => {
        if (res.success) {
          this.items.set((res.data?.results ?? []).map(u => ({
            id:             u.id ?? 0,
            nombreCompleto: u.nombreCompleto ?? '',
            email:          u.email ?? '',
            usuarioNombre:  u.usuarioNombre ?? '',
            rolNombre:      u.rol?.sRol ?? '',
          })));
          this.currentPage.set(res.data?.currentPage ?? 1);
          this.pageSize.set(res.data?.pageSize ?? 10);
          this.totalCount.set(res.data?.totalCount ?? 0);
          this.totalPages.set(res.data?.totalPages ?? 0);
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
    this.authApiService.getUsuarioById(id).subscribe({
      next: res => {
        if (res.success) {
          this.usuarioSeleccionado = { ...res.data } as Partial<UsuarioDto>;
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
    const payload = usuario as any;

    const request$ = isUpdate
      ? this.authApiService.updateUsuario(id, {
          id,
          nombreCompleto: payload.nombreCompleto,
          email:          payload.email,
          usuarioNombre:  payload.usuarioNombre,
          rolId:          payload.rolId,
          contrasena:     payload.contrasenia ?? null,
        } as UsuarioUpdateDto)
      : this.authApiService.createUsuario({
          nombreCompleto: payload.nombreCompleto,
          email:          payload.email,
          usuarioNombre:  payload.usuarioNombre,
          rolId:          payload.rolId,
          contrasena:     payload.contrasenia ?? '',
        } as UsuarioCreateDto);

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
    this.authApiService.deleteUsuario(this.usuarioAEliminar.id).subscribe({
      next: res => {
        if (res?.success !== false) {
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
