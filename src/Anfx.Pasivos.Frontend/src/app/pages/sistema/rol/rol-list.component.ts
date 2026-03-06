import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../../services/layout.service';
import { RolService } from '../../../services/sistema/rol.service';
import { UtilsService } from '../../../services/utils.service';
import { RolFormComponent } from './rol-form.component';
import { RolDto, RolPageQueryDto } from '../../../../types/sistema/rol.dto';

@Component({
  selector: 'app-rol-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RolFormComponent],
  templateUrl: './rol-list.component.html'
})
export class RolListComponent implements OnInit {
  private rolService = inject(RolService);
  private layoutService = inject(LayoutService);
  private utilsService = inject(UtilsService);

  items = signal<RolDto[]>([]);
  loading = signal<boolean>(false);
  query: RolPageQueryDto = { q: '', page: 1, size: 10, sortby: []};

  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);

  rolSeleccionado: Partial<RolDto> = { id: 0, sRol: '', descripcion: '' };
  mostrandoFormulario = signal<boolean>(false);
  rolAEliminar: RolDto | null = null;

  // Exponer Math para usar en el template
  Math = Math;

  ngOnInit(): void {
    this.layoutService.setTitle('Roles');
    this.load();
  }

  onSearch() {
    this.resetSortingClass();

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
    this.rolService.getAll(this.query).subscribe({
      next: response => {
        if (response.success) {
          this.items.set(response.data.results);
          this.currentPage.set(response.data.currentPage);
          this.pageSize.set(response.data.pageSize);
          this.totalCount.set(response.data.totalCount);
          this.totalPages.set(response.data.totalPages);
        } else {
          this.items.set([]);
          this.currentPage.set(this.query.page || 1);
          this.pageSize.set(this.query.size || 10);
          this.totalCount.set(0);
          this.totalPages.set(0);

          if (response.errors && response.errors.length > 0) {
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            this.utilsService.showNotification('Error', 'Error al cargar roles', 'error');
          }
        }
        this.loading.set(false);
      },
      error: (httpError) => {
        this.items.set([]);
        this.currentPage.set(this.query.page || 1);
        this.pageSize.set(this.query.size || 10);
        this.totalCount.set(0);
        this.totalPages.set(0);
        this.loading.set(false);
        this.utilsService.showNotification('Error', 'Error de conexión al cargar roles', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  mostrarFormularioNuevo() {
    this.rolSeleccionado = {
      id: 0,
      sRol: '',
      descripcion: ''
    };
    this.mostrandoFormulario.set(true);
  }

  editarRol(id: number) {
    this.rolService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.rolSeleccionado = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          if (response.errors && response.errors.length > 0) {
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            this.utilsService.showNotification('Error', 'Error al cargar el rol', 'error');
          }
        }
      },
      error: (httpError) => {
        this.utilsService.showNotification('Error', 'Error de conexión al cargar el rol', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  onGuardarRol(rol: any) {
    if ('id' in rol && rol.id && typeof rol.id === 'number') {
      this.rolService.update(rol.id, rol).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.mostrandoFormulario.set(false);
          } else {
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al actualizar el rol', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al actualizar el rol', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    } else {
      this.rolService.create(rol).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.mostrandoFormulario.set(false);
          } else {
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al crear el rol', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al crear el rol', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    }
  }

  onCancelarEdicion() {
    this.mostrandoFormulario.set(false);
    this.rolSeleccionado = {
      id: 0,
      sRol: '',
      descripcion: ''
    };
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.rolSeleccionado = {
      id: 0,
      sRol: '',
      descripcion: ''
    };
  }

  edit(id: number) {
    this.editarRol(id);
  }

  delete(id: number) {
    const rol = this.items().find(r => r.id === id);
    if (rol) {
      this.rolAEliminar = rol;
      this.mostrarModalEliminacion();
    }
  }

  private mostrarModalEliminacion() {
    const modal = document.getElementById('confirmDeleteModal');
    if (modal) {
      const bootstrapModal = new (window as any).bootstrap.Modal(modal);
      bootstrapModal.show();
    }
  }

  confirmarEliminacion() {
    if (this.rolAEliminar) {
      this.rolService.delete(this.rolAEliminar.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.cerrarModalEliminacion();
          } else {
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al eliminar el rol', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar el rol', 'error');
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
    this.rolAEliminar = null;
  }

  cancelarEliminacion() {
    this.cerrarModalEliminacion();
  }

  private resetSortingClass(): void{
    const elementos = document.querySelectorAll('.ti-arrow-down');

    elementos?.forEach((elemento) => {
      elemento.classList.remove('ti-arrow-down');
      elemento.classList.add('ti-arrow-up');
    });

    this.query.sortby = [];
  }

  sortBy(event: MouseEvent): void{

    const id = (event.target as HTMLElement).id;
    let element = document.getElementById(id);
    let orderByDesc = false;

    if(element?.classList.contains('ti-arrow-up'))
    {
      element?.classList.remove('ti-arrow-up');
      element?.classList.add('ti-arrow-down');

      orderByDesc = true;
    }
    else
    {
      element?.classList.add('ti-arrow-up');
      element?.classList.remove('ti-arrow-down');

      orderByDesc = false;
    }

    const indexToRemove = this.query.sortby?.findIndex(param =>
      param.column === id
    );

    if (indexToRemove !== undefined && indexToRemove > -1) {
      this.query.sortby?.splice(indexToRemove, 1);
    }
    this.query.sortby?.push({ column: id, desc: orderByDesc });

    this.query.page = 1;
    this.load();
  }
}
