import { Component, inject, signal, ViewChild, DestroyRef, effect } from '@angular/core';
import { AuthService as AuthApiService } from '../../../../api/services/auth.service';
import { UsuarioCreateDto, UsuarioUpdateDto } from '../../../../api/models/models';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import {
  UsuarioItemDto,
  UsuarioDto,
  UsuarioPageQueryDto,
  UsuarioFormData,
} from '../../../../types/sistema/usuario.dto';
import { UtilsService } from '../../../services/utils.service';
import { UsuarioFormComponent } from './usuario-form.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import {
  TableColumn,
  TableAction,
  TableActionEvent,
  TableSortEvent,
} from '../../../shared/components/generic-table/table-column.model';
import { FilterActivoComponent } from '../../../shared/components/filter-activo/filter-activo.component';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';
import { TableDataSource } from 'src/app/core/datasource/table.datasource';
import { map } from 'rxjs';

@Component({
  selector: 'app-usuario-list',
  standalone: true,
  imports: [
    UsuarioFormComponent,
    GenericTableComponent,
    FilterActivoComponent,
    ConfirmModalComponent,
  ],
  templateUrl: './usuario-list.component.html',
})
export class UsuarioListComponent {

  private readonly authApiService = inject(AuthApiService);
  private readonly utilsService = inject(UtilsService);
  readonly ds = new TableDataSource<UsuarioItemDto>(
    (q) =>
      this.authApiService.getUsuariosPaginados(q.q, q.page, q.size, q.activo, q.sortBy,q.sortDir).pipe(
        map((res) => ({
          ...res,
          data: res.data
            ? {
                ...res.data,
                results: (res.data.results ?? []).map(
                  (u) =>
                    ({
                      id: u.id ?? 0,
                      nombreCompleto: u.nombreCompleto ?? '',
                      email: u.email ?? '',
                      usuarioNombre: u.usuarioNombre ?? '',
                      rolNombre: u.rol?.sRol ?? '',
                    }) as UsuarioItemDto,
                ),
              }
            : undefined,
        })),
      ),
    inject(DestroyRef),
  );

  // ── Tabla ────────────────────────────────────────────────────
  columns: TableColumn[] = [
    { key: 'id', header: 'ID', type: 'number', sortable: true, maxWidth: 80 },
    { key: 'nombreCompleto', header: 'Nombre', type: 'text', sortable: true, maxWidth: 220 },
    { key: 'email', header: 'Email', type: 'text', sortable: true, maxWidth: 200, hideBelow: 'sm' },
    { key: 'usuarioNombre', header: 'Usuario', type: 'text', sortable: true, hideBelow: 'md' },
    { key: 'rolNombre', header: 'Rol', type: 'text', hideBelow: 'lg' },
  ];

  actions: TableAction[] = [
    { id: 'edit', label: 'Editar', icon: 'fa-solid fa-user-pen', variant: 'edit' },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-user-minus', variant: 'delete' },
  ];
  query: UsuarioPageQueryDto = { q: '', page: 1, size: 10, activo: true };

  // ── Formulario ───────────────────────────────────────────────
  mostrandoFormulario = signal(false);
  usuarioSeleccionado: Partial<UsuarioDto> = {};

  // ── Modal eliminación ─────────────────────────────────────────
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;
  usuarioAEliminar: UsuarioItemDto | null = null;

  constructor() {
    effect(() => {
      const e = this.ds.error();
      if (e) this.utilsService.showNotification('Error', e, 'error');
    });
    this.ds.load();
  }

  // ── Eventos del GenericTable ──────────────────────────────────

  onAction(event: TableActionEvent) {
    switch (event.action) {
      case 'edit':
        this.editarUsuario((event.row as UsuarioItemDto).id);
        break;
      case 'delete':
        this.iniciarEliminacion(event.row as UsuarioItemDto);
        break;
    }
  }

  onSearch(q: string) {
    this.ds.search(q);
  }
  onSort(e: TableSortEvent) {
    this.ds.sort(e.column, e.direction as 'asc' | 'desc');
  }
  onPageNext() {
    this.ds.nextPage();
  }
  onPagePrev() {
    this.ds.prevPage();
  }
  onActivoChange(val: boolean) {
    this.ds.setFilter({ activo: val });
  }

  onNuevo() {
    this.usuarioSeleccionado = {
      id: 0,
      nombreCompleto: '',
      email: '',
      usuarioNombre: '',
      activo: true,
      rolId: 0,
    };
    this.mostrandoFormulario.set(true);
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.usuarioSeleccionado = {};
  }

  private editarUsuario(id: number) {
    this.authApiService.getUsuarioById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.usuarioSeleccionado = { ...res.data } as Partial<UsuarioDto>;
          this.mostrandoFormulario.set(true);
        } else {
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? res.message ?? 'Error al cargar el usuario',
            'error',
          );
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification(
            'Error',
            'Error de conexión al cargar el usuario',
            'error',
          );
      },
    });
  }

  onGuardarUsuario(usuario: UsuarioFormData) {
    const id = this.usuarioSeleccionado.id;
    const isUpdate = id != null && id > 0;
    const request$ = isUpdate
      ? this.authApiService.updateUsuario(id, {
          id,
          nombreCompleto: usuario.nombreCompleto,
          email: usuario.email,
          usuarioNombre: usuario.usuarioNombre,
          rolId: usuario.rolId,
          contrasena: usuario.Contrasenia ?? null, // ← tipado, Contrasenia con C mayúscula
        } as UsuarioUpdateDto)
      : this.authApiService.createUsuario({
          nombreCompleto: usuario.nombreCompleto!,
          email: usuario.email!,
          usuarioNombre: usuario.usuarioNombre!,
          rolId: usuario.rolId!,
          contrasena: usuario.Contrasenia ?? '', // ← tipado
        } as UsuarioCreateDto);

    request$.subscribe({
      next: (res) => {
        if (res.success) {
          this.ds.reload();
          this.mostrandoFormulario.set(false);
        } else {
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? res.message ?? 'Error al guardar el usuario',
            'error',
          );
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification(
            'Error',
            'Error de conexión al guardar el usuario',
            'error',
          );
      },
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
      next: (res) => {
        if (res?.success === false) {
          this.utilsService.showNotification(
            'Error',
            res.errors?.[0] ?? res.message ?? 'Error al eliminar',
            'error',
          );
        } else {
          this.ds.reload();
          this.confirmModal.hide();
          this.usuarioAEliminar = null;
        }
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err))
          this.utilsService.showNotification(
            'Error',
            'Error de conexión al eliminar el usuario',
            'error',
          );
      },
    });
  }

  cancelarEliminacion() {
    this.usuarioAEliminar = null;
  }
}
