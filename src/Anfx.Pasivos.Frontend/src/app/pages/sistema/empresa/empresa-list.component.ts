import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmpresaService } from '../../../services/sistema/empresa.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';
import { EmpresaFormComponent } from './empresa-form.component';
import { EmpresaDto } from '../../../../types/sistema/empresa.dto';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from '../../../shared/components/generic-table/table-column.model';
import { CardComponent } from '../../../shared/components/card/card.component';

@Component({
  selector: 'app-empresa-list',
  standalone: true,
  imports: [CommonModule, EmpresaFormComponent, ConfirmModalComponent, GenericTableComponent, CardComponent],
  templateUrl: './empresa-list.component.html'
})
export class EmpresaListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly empresaService = inject(EmpresaService);
  private readonly utilsService   = inject(UtilsService);

  items       = signal<EmpresaDto[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  sortColumn:    string | null = null;
  sortDirection: SortDirection = 'asc';
  searchValue = '';

  mostrandoFormulario = signal(false);
  empresaSeleccionada: Partial<EmpresaDto> = this.emptyEmpresa();
  empresaAEliminar: EmpresaDto | null = null;

  private q    = '';
  private page = 1;

  columns: TableColumn[] = [
    { key: 'id',            header: 'ID',             type: 'number', sortable: true  },
    { key: 'sEmpresa',      header: 'Empresa',         type: 'text',   sortable: true  },
    { key: 'rFC',           header: 'RFC',             type: 'text',   sortable: false },
    { key: 'razonSocial',   header: 'Razón Social',    type: 'text',   sortable: true  },
    { key: 'representante', header: 'Representante',   type: 'text',   sortable: false },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',   icon: 'fa-solid fa-pen-clip',  variant: 'edit'   },
    { id: 'delete', label: 'Eliminar', icon: 'fa-solid fa-trash-can', variant: 'delete' },
  ];

  ngOnInit() {
    this.load();
  }

  onSearch(value: string) {
    this.q           = value;
    this.searchValue = value;
    this.page        = 1;
    this.load();
  }

  onSort(event: TableSortEvent) {
    this.sortColumn    = event.column;
    this.sortDirection = event.direction;
    this.page = 1;
    this.load();
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) { this.page++; this.load(); }
  }

  prevPage() {
    if (this.page > 1) { this.page--; this.load(); }
  }

  onNuevo() {
    this.empresaSeleccionada = this.emptyEmpresa();
    this.mostrandoFormulario.set(true);
  }

  onAction(event: TableActionEvent<EmpresaDto>) {
    if (event.action === 'edit')   this.editarEmpresa(event.row.id);
    if (event.action === 'delete') this.delete(event.row.id);
  }

  onGuardarEmpresa(empresa: any) {
    const isUpdate = this.empresaSeleccionada?.id && this.empresaSeleccionada.id > 0;
    const request$ = isUpdate
      ? this.empresaService.update(this.empresaSeleccionada.id!, empresa)
      : this.empresaService.create(empresa);

    request$.subscribe({
      next: (response) => {
        if (response.success) {
          this.load();
          this.mostrandoFormulario.set(false);
          const msg = isUpdate ? 'Empresa actualizada correctamente' : 'Empresa creada correctamente';
          this.utilsService.showNotification('Éxito', msg, 'success');
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al guardar la empresa';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al guardar la empresa', 'error'); }
    });
  }

  onCancelarEdicion() {
    this.volverALista();
  }

  volverALista() {
    this.mostrandoFormulario.set(false);
    this.empresaSeleccionada = this.emptyEmpresa();
  }

  confirmarEliminacion() {
    if (!this.empresaAEliminar) return;
    this.empresaService.delete(this.empresaAEliminar.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.load();
          this.confirmModal.hide();
          this.empresaAEliminar = null;
          this.utilsService.showNotification('Éxito', 'Empresa eliminada correctamente', 'success');
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al eliminar la empresa';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al eliminar la empresa', 'error'); }
    });
  }

  cancelarEliminacion() {
    this.empresaAEliminar = null;
  }

  private editarEmpresa(id: number) {
    this.empresaService.getById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.empresaSeleccionada = { ...response.data };
          this.mostrandoFormulario.set(true);
        } else {
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar la empresa';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => { if (!wasHandledByInterceptor(err)) this.utilsService.showNotification('Error', 'Error de conexión al cargar la empresa', 'error'); }
    });
  }

  private delete(id: number) {
    const empresa = this.items().find(e => e.id === id);
    if (!empresa) return;
    this.empresaAEliminar = empresa;
    this.confirmModal.show();
  }

  private load() {
    this.loading.set(true);
    this.empresaService.getAll({ q: this.q, page: this.page, size: this.pageSize() }).subscribe({
      next: (response) => {
        if (response.success) {
          const size  = response.data.pageSize  ?? 10;
          const total = response.data.totalCount ?? 0;
          this.items.set(response.data.results ?? []);
          this.currentPage.set(response.data.currentPage ?? this.page);
          this.pageSize.set(size);
          this.totalCount.set(total);
          this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
        } else {
          this.resetPagination();
          const msg = response.errors?.[0] ?? response.message ?? 'Error al cargar empresas';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.resetPagination();
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar empresas', 'error');
        }
      }
    });
  }

  private resetPagination() {
    this.items.set([]);
    this.currentPage.set(this.page);
    this.pageSize.set(10);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }

  private emptyEmpresa(): Partial<EmpresaDto> {
    return {
      id: 0, sEmpresa: '', rFC: '', razonSocial: '', telefono: '',
      representante: '', avisosEstadodeCuenta: '', advertenciasEstadodeCuenta: '',
      aclaracionesEstadodeCuenta: '', usaDesembolso: false, pasivo: false,
      tipoDireccionId: 0, calle: '', numExterior: '', numInterior: ''
    };
  }
}
