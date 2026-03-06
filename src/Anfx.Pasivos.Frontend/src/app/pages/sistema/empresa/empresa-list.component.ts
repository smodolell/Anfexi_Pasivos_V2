import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../../services/layout.service';
import { EmpresaService } from '../../../services/sistema/empresa.service';
import { UtilsService } from '../../../services/utils.service';
import { EmpresaFormComponent } from './empresa-form.component';
import { EmpresaDto, EmpresaPageQueryDto } from '../../../../types/sistema/empresa.dto';

@Component({
  selector: 'app-empresa-list',
  standalone: true,
  imports: [CommonModule, FormsModule, EmpresaFormComponent],
  templateUrl: './empresa-list.component.html'
})
export class EmpresaListComponent implements OnInit {
  private empresaService = inject(EmpresaService);
  private layoutService = inject(LayoutService);
  private utilsService = inject(UtilsService);

  items = signal<EmpresaDto[]>([]);
  loading = signal<boolean>(false);
  query: EmpresaPageQueryDto = { q: '', page: 1, size: 10 };

  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);

  empresaSeleccionada: Partial<EmpresaDto> = {
    id: 0,
    sEmpresa: '',
    rFC: '',
    razonSocial: '',
    telefono: '',
    representante: '',
    avisosEstadodeCuenta: '',
    advertenciasEstadodeCuenta: '',
    aclaracionesEstadodeCuenta: '',
    usaDesembolso: false,
    pasivo: false,
    tipoDireccionId: 0,
    calle: '',
    numExterior: '',
    numInterior: ''
  };
  mostrandoFormulario = signal<boolean>(false);
  empresaAEliminar: EmpresaDto | null = null;

  // Exponer Math para usar en el template
  Math = Math;

  ngOnInit(): void {
    this.layoutService.setTitle('Empresas');
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
    this.empresaService.getAll(this.query).subscribe({
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
            this.utilsService.showNotification('Error', 'Error al cargar empresas', 'error');
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
        this.utilsService.showNotification('Error', 'Error de conexión al cargar empresas', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  mostrarFormularioNuevo() {
    this.empresaSeleccionada = {
      id: 0,
      sEmpresa: '',
      rFC: '',
      razonSocial: '',
      telefono: '',
      representante: '',
      avisosEstadodeCuenta: '',
      advertenciasEstadodeCuenta: '',
      aclaracionesEstadodeCuenta: '',
      usaDesembolso: false,
      pasivo: false,
      tipoDireccionId: undefined,
      calle: '',
      numExterior: '',
      numInterior: ''
    };
    this.mostrandoFormulario.set(true);
  }

  editarEmpresa(id: number) {
    this.empresaService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.empresaSeleccionada = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          if (response.errors && response.errors.length > 0) {
            this.utilsService.showNotification('Error', response.errors[0], 'error');
          } else if (response.message) {
            this.utilsService.showNotification('Error', response.message, 'error');
          } else {
            this.utilsService.showNotification('Error', 'Error al cargar la empresa', 'error');
          }
        }
      },
      error: (httpError) => {
        this.utilsService.showNotification('Error', 'Error de conexión al cargar la empresa', 'error');
        console.error('Error HTTP:', httpError);
      }
    });
  }

  onGuardarEmpresa(empresa: any) {
    if (this.empresaSeleccionada && this.empresaSeleccionada.id && this.empresaSeleccionada.id > 0) {
      this.empresaService.update(this.empresaSeleccionada.id, empresa).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.mostrandoFormulario.set(false);
            this.utilsService.showNotification('Éxito', 'Empresa actualizada correctamente', 'success');
          } else {
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al actualizar la empresa', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al actualizar la empresa', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    } else {
      this.empresaService.create(empresa).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.mostrandoFormulario.set(false);
            this.utilsService.showNotification('Éxito', 'Empresa creada correctamente', 'success');
          } else {
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al crear la empresa', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al crear la empresa', 'error');
          console.error('Error HTTP:', httpError);
        }
      });
    }
  }

  onCancelarEdicion() {
    this.mostrandoFormulario.set(false);
    this.empresaSeleccionada = {
      id: 0,
      sEmpresa: '',
      rFC: '',
      razonSocial: '',
      telefono: '',
      representante: '',
      avisosEstadodeCuenta: '',
      advertenciasEstadodeCuenta: '',
      aclaracionesEstadodeCuenta: '',
      usaDesembolso: false,
      pasivo: false,
      tipoDireccionId: 0,
      calle: '',
      numExterior: '',
      numInterior: ''
    };
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.empresaSeleccionada = {
      id: 0,
      sEmpresa: '',
      rFC: '',
      razonSocial: '',
      telefono: '',
      representante: '',
      avisosEstadodeCuenta: '',
      advertenciasEstadodeCuenta: '',
      aclaracionesEstadodeCuenta: '',
      usaDesembolso: false,
      pasivo: false,
      tipoDireccionId: 0,
      calle: '',
      numExterior: '',
      numInterior: ''
    };
  }

  edit(id: number) {
    this.editarEmpresa(id);
  }

  delete(id: number) {
    const empresa = this.items().find(e => e.id === id);
    if (empresa) {
      this.empresaAEliminar = empresa;
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
    if (this.empresaAEliminar) {
      this.empresaService.delete(this.empresaAEliminar.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.load();
            this.cerrarModalEliminacion();
            this.utilsService.showNotification('Éxito', 'Empresa eliminada correctamente', 'success');
          } else {
            if (response.errors && response.errors.length > 0) {
              this.utilsService.showNotification('Error', response.errors[0], 'error');
            } else if (response.message) {
              this.utilsService.showNotification('Error', response.message, 'error');
            } else {
              this.utilsService.showNotification('Error', 'Error al eliminar la empresa', 'error');
            }
          }
        },
        error: (httpError) => {
          this.utilsService.showNotification('Error', 'Error de conexión al eliminar la empresa', 'error');
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
    this.empresaAEliminar = null;
  }

  cancelarEliminacion() {
    this.cerrarModalEliminacion();
  }
}
