import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../../services/layout.service';
import { TipoDireccionService } from '../../../services/catalogos/tipodireccion.service';
import { TipoDireccionDto, TipoDireccionPageQueryDto } from '../../../../types/catalogos/tipodireccion.dto';
import { UtilsService } from '../../../services/utils.service';
import { TipoDireccionFormComponent } from './tipodireccion-form.component';

@Component({
  selector: 'app-tipodireccion-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TipoDireccionFormComponent],
  templateUrl: './tipodireccion-list.component.html'
})
export class TipoDireccionListComponent implements OnInit {
  private tipoDireccionService = inject(TipoDireccionService);
  private layoutService = inject(LayoutService);
  private utilsService = inject(UtilsService);

  items = signal<TipoDireccionDto[]>([]);
  loading = signal<boolean>(false);
  query: TipoDireccionPageQueryDto = { q: '', page: 1, size: 10 };

  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);

  tipoDireccionSeleccionado: Partial<TipoDireccionDto> = {};
  mostrandoFormulario = signal<boolean>(false);
  tipoDireccionAEliminar: TipoDireccionDto | null = null;

  // Exponer Math para usar en el template
  Math = Math;

  ngOnInit(): void {
    this.layoutService.setTitle('Administración de Tipos de Dirección');
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
    this.tipoDireccionService.getAll(this.query).subscribe({
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
            this.utilsService.showNotification('Error', 'Error al cargar tipos de dirección', 'error');
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
        this.utilsService.showNotification('Error', 'Error de conexión al cargar tipos de dirección', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  mostrarFormularioNuevo() {
    this.tipoDireccionSeleccionado = {
      id: 0,
      sTipoDireccion: ''
    };
    this.mostrandoFormulario.set(true);
  }

  editarTipoDireccion(id: number) {
    this.tipoDireccionService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.tipoDireccionSeleccionado = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          if (response.errors && response.errors.length > 0) {
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            this.utilsService.showNotification('Error', 'Error al cargar el tipo de dirección', 'error');
          }
        }
      },
      error: (httpError) => {
        this.utilsService.showNotification('Error', 'Error de conexión al cargar el tipo de dirección', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  onGuardarTipoDireccion(tipoDireccion: any) {
    if ('Id' in tipoDireccion && tipoDireccion.Id && typeof tipoDireccion.Id === 'number') {
      this.tipoDireccionService.update(tipoDireccion.Id, tipoDireccion).subscribe({
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
              this.utilsService.showNotification('Error', 'Error al actualizar el tipo de dirección', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al actualizar el tipo de dirección', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    } else {
      this.tipoDireccionService.create(tipoDireccion).subscribe({
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
              this.utilsService.showNotification('Error', 'Error al crear el tipo de dirección', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al crear el tipo de dirección', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    }
  }

  onCancelarEdicion() {
    this.mostrandoFormulario.set(false);
    this.tipoDireccionSeleccionado = {
      id: 0,
      sTipoDireccion: ''
    };
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.tipoDireccionSeleccionado = {
      id: 0,
      sTipoDireccion: ''
    };
  }

  edit(id: number) {
    this.editarTipoDireccion(id);
  }

  delete(id: number) {
    const tipoDireccion = this.items().find(t => t.id === id);
    if (tipoDireccion) {
      this.tipoDireccionAEliminar = tipoDireccion;
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
    if (this.tipoDireccionAEliminar) {
      this.tipoDireccionService.delete(this.tipoDireccionAEliminar.id).subscribe({
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
              this.utilsService.showNotification('Error', 'Error al eliminar el tipo de dirección', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar el tipo de dirección', 'error');
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
    this.tipoDireccionAEliminar = null;
  }

  cancelarEliminacion() {
    this.cerrarModalEliminacion();
  }

  onExportar() {
    this.loading.set(true);
    this.tipoDireccionService.exportar(this.query.q).subscribe({
      next: (blob) => {
        // Crear un enlace temporal para descargar el archivo
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;

        // Generar nombre de archivo con fecha y hora
        const fecha = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
        const nombreArchivo = `tipos_direcciones_${fecha}.xlsx`;

        link.download = nombreArchivo;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);

        this.loading.set(false);
        this.utilsService.showNotification('Éxito', 'Archivo exportado correctamente', 'success');
      },
      error: (httpError) => {
        this.loading.set(false);
        this.utilsService.showNotification('Error', 'Error al exportar el archivo', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }
}
