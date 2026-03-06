import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../../services/layout.service';
import { ColoniaService } from '../../../services/catalogos/colonia.service';
import { ColoniaDto, ColoniaPageQueryDto } from '../../../../types/catalogos/colonia.dto';
import { UtilsService } from '../../../services/utils.service';
import { ColoniaFormComponent } from './colonia-form.component';

@Component({
  selector: 'app-colonia-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ColoniaFormComponent],
  templateUrl: './colonia-list.component.html'
})
export class ColoniaListComponent implements OnInit {
  private coloniaService = inject(ColoniaService);
  private layoutService = inject(LayoutService);
  private utilsService = inject(UtilsService);

  items = signal<ColoniaDto[]>([]);
  loading = signal<boolean>(false);
  query: ColoniaPageQueryDto = { q: '', page: 1, size: 10 };
  
  // metadata de paginación
  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);

  // Nuevas propiedades para el formulario
  coloniaSeleccionada: Partial<ColoniaDto> = {};
  mostrandoFormulario = signal<boolean>(false);

  // Propiedades para el modal de eliminación
  coloniaAEliminar: ColoniaDto | null = null;
  
  // Exponer Math para usar en el template
  Math = Math;

  ngOnInit(): void {
    this.layoutService.setTitle('Administración de Colonias');
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
    this.coloniaService.getAll(this.query).subscribe({
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
            this.utilsService.showNotification('Error', 'Error al cargar colonias', 'error');
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
        this.utilsService.showNotification('Error', 'Error de conexión al cargar colonias', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  // Nuevos métodos para el formulario
  mostrarFormularioNuevo() {
    this.coloniaSeleccionada = {
      id: 0,
      sColonia: '',
      estado: '',
      municipio: '',
      codigoPostal: ''
    };
    this.mostrandoFormulario.set(true);
  }

  editarColonia(id: number) {
    this.coloniaService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.coloniaSeleccionada = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          // Error de la API
          if (response.errors && response.errors.length > 0) {
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            this.utilsService.showNotification('Error', 'Error al cargar la colonia', 'error');
          }
        }
      },
      error: (httpError) => {
        this.utilsService.showNotification('Error', 'Error de conexión al cargar la colonia', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  onGuardarColonia(colonia: any) {
    if ('Id' in colonia && colonia.Id && typeof colonia.Id === 'number') {
      // Actualizar colonia existente
      this.coloniaService.update(colonia.Id, colonia).subscribe({
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
              this.utilsService.showNotification('Error', 'Error al actualizar la colonia', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al actualizar la colonia', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    } else {
      // Crear nueva colonia
      this.coloniaService.create(colonia).subscribe({
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
              this.utilsService.showNotification('Error', 'Error al crear la colonia', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al crear la colonia', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    }
  }

  onCancelarEdicion() {
    this.mostrandoFormulario.set(false);
    this.coloniaSeleccionada = {
      id: 0,
      sColonia: '',
      estado: '',
      municipio: '',
      codigoPostal: ''
    };
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.coloniaSeleccionada = {
      id: 0,
      sColonia: '',
      estado: '',
      municipio: '',
      codigoPostal: ''
    };
  }

  edit(id: number) {
    this.editarColonia(id);
  }

  delete(id: number) {
    // Buscar la colonia por ID para mostrar su nombre en el modal
    const colonia = this.items().find(c => c.id === id);
    if (colonia) {
      this.coloniaAEliminar = colonia;
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
    if (this.coloniaAEliminar) {
      this.coloniaService.delete(this.coloniaAEliminar.id).subscribe({
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
              this.utilsService.showNotification('Error', 'Error al eliminar la colonia', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar la colonia', 'error');
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
    this.coloniaAEliminar = null;
  }

  cancelarEliminacion() {
    this.cerrarModalEliminacion();
  }

  onExportar() {
    this.loading.set(true);
    this.coloniaService.exportar(this.query.q).subscribe({
      next: (blob) => {
        // Crear un enlace temporal para descargar el archivo
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        
        // Generar nombre de archivo con fecha y hora
        const fecha = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
        const nombreArchivo = `colonias_${fecha}.xlsx`;
        
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
