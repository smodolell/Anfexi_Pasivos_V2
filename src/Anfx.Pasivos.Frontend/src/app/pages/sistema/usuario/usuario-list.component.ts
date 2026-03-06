import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../../services/layout.service';
import { UsuarioService } from '../../../services/sistema/usuario.service';
import { UsuarioItemDto, UsuarioDto, CreateUsuarioDto, UpdateUsuarioDto, UsuarioPageQueryDto } from '../../../../types/sistema/usuario.dto';
import { UtilsService } from '../../../services/utils.service';
import { UsuarioFormComponent } from './usuario-form.component';

@Component({
  selector: 'app-usuario-list',
  standalone: true,
  imports: [CommonModule, FormsModule, UsuarioFormComponent],
  templateUrl: './usuario-list.component.html'
})
export class UsuarioListComponent implements OnInit {
  private usuarioService = inject(UsuarioService);
  private layoutService = inject(LayoutService);
  private utilsService = inject(UtilsService);

  items = signal<UsuarioItemDto[]>([]);
  loading = signal<boolean>(false);
  query: UsuarioPageQueryDto = { q: '', page: 1, size: 10, activo: true };

  // metadata de paginación
  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);

  // Nuevas propiedades para el formulario
  usuarioSeleccionado: Partial<UsuarioDto> = {};
  mostrandoFormulario = signal<boolean>(false);

  // Propiedades para el modal de eliminación
  usuarioAEliminar: UsuarioItemDto | null = null;

  ngOnInit(): void {
    this.layoutService.setTitle('Administración de Usuarios');
    this.load();
  }

  onSearch() {
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

  private load() {
    this.loading.set(true);
    this.usuarioService.getAll(this.query).subscribe({
      next: response => {
        if (response.success) {
          // Éxito: establecer los datos
          this.items.set(response.data.results);
          this.currentPage.set(response.data.currentPage);
          this.pageSize.set(response.data.pageSize);
          this.totalCount.set(response.data.totalCount);
          this.totalPages.set(response.data.totalPages);
        } else {
          // Error de la API: mostrar errores específicos
          this.items.set([]);
          this.currentPage.set(this.query.page || 1);
          this.pageSize.set(this.query.size || 10);
          this.totalCount.set(0);
          this.totalPages.set(0);

          if (response.errors && response.errors.length > 0) {
            // Mostrar el primer error
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            // Mostrar mensaje de error general
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            // Mensaje de error por defecto
            this.utilsService.showNotification('Error', 'Error al cargar usuarios', 'error');
          }
        }
        this.loading.set(false);
      },
      error: (httpError) => {
        // Error de HTTP (conexión, servidor, etc.)
        this.items.set([]);
        this.currentPage.set(this.query.page || 1);
        this.pageSize.set(this.query.size || 10);
        this.totalCount.set(0);
        this.totalPages.set(0);
        this.loading.set(false);

        // Mostrar error de conexión
        this.utilsService.showNotification('Error', 'Error de conexión al cargar usuarios', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  // Nuevos métodos para el formulario
  mostrarFormularioNuevo() {
    this.usuarioSeleccionado = {
      id: 0,
      nombreCompleto: '',
      email: '',
      usuarioNombre: '',
      activo: true,
      rolId: 0
    };
    this.mostrandoFormulario.set(true);
  }

  editarUsuario(id: number) {
    this.usuarioService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.usuarioSeleccionado = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          // Error de la API
          if (response.errors && response.errors.length > 0) {
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            this.utilsService.showNotification('Error', 'Error al cargar el usuario', 'error');
          }
        }
      },
      error: (httpError) => {
        this.utilsService.showNotification('Error', 'Error de conexión al cargar el usuario', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  onGuardarUsuario(usuario: CreateUsuarioDto | UpdateUsuarioDto) {
    if ('id' in usuario && usuario.id && typeof usuario.id === 'number') {
      // Actualizar usuario existente
      this.usuarioService.update(usuario.id, usuario).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.mostrandoFormulario.set(false);
          } else {
            // Error de la API
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al actualizar el usuario', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al actualizar el usuario', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    } else {
      // Crear nuevo usuario
      this.usuarioService.create(usuario as CreateUsuarioDto).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.mostrandoFormulario.set(false);
          } else {
            // Error de la API
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al crear el usuario', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al crear el usuario', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    }
  }

  onCancelarEdicion() {
    this.mostrandoFormulario.set(false);
    this.usuarioSeleccionado = {
      id: 0,
      nombreCompleto: '',
      email: '',
      usuarioNombre: '',
      activo: true,
      rolId: 0
    };
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.usuarioSeleccionado = {
      id: 0,
      nombreCompleto: '',
      email: '',
      usuarioNombre: '',
      activo: true,
      rolId: 0
    };
  }

  edit(id: number) {
    this.editarUsuario(id);
  }

  delete(id: number) {
    // Buscar el usuario por ID para mostrar su nombre en el modal
    const usuario = this.items().find(u => u.id === id);
    if (usuario) {
      this.usuarioAEliminar = usuario;
      this.mostrarModalEliminacion();
    }
  }

  private mostrarModalEliminacion() {
    // Usar Bootstrap Modal API
    const modal = document.getElementById('confirmDeleteModal');
    if (modal) {
      const bootstrapModal = new (window as any).bootstrap.Modal(modal);
      bootstrapModal.show();
    }
  }

  confirmarEliminacion() {
    if (this.usuarioAEliminar) {
      this.usuarioService.delete(this.usuarioAEliminar.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.cerrarModalEliminacion();
          } else {
            // Error de la API
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al eliminar el usuario', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar el usuario', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    }
  }

  private cerrarModalEliminacion() {
    const modal = document.getElementById('confirmDeleteModal');
    if (modal) {
      const bootstrapModal = (window as any).bootstrap.Modal.getInstance(modal);
      if (bootstrapModal) {
        bootstrapModal.hide();
      }
    }
    this.usuarioAEliminar = null;
  }

  cancelarEliminacion() {
    this.cerrarModalEliminacion();
  }
}


