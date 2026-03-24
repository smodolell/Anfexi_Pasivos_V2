import { Component, OnInit, inject, signal, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ConfiguracionesService } from '../../../../api/services/configuraciones.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { FondeadorListItemDto } from '../../../../api/models/fondeadorListItemDto';
import { UtilsService } from '../../../services/utils.service';
import { GenericTableComponent } from '../../../shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '../../../shared/components/generic-table/table-column.model';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-fondeador-list',
  standalone: true,
  imports: [GenericTableComponent, ConfirmModalComponent],
  templateUrl: './fondeador-list.component.html'
})
export class FondeadorListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private service      = inject(ConfiguracionesService);
  private router       = inject(Router);
  private utilsService = inject(UtilsService);

  // ── Tabla ────────────────────────────────────────────────────
  columns: TableColumn[] = [
    { key: 'id',                  header: 'ID',                  type: 'number', maxWidth: 80 },
    { key: 'titulo',              header: 'Fondeador',           type: 'text',   sortable: true },
    { key: 'claveCuentaContable', header: 'Clave Cta. Contable', type: 'text',   hideBelow: 'md' },
    { key: 'lineasCredito',       header: 'Líneas',              type: 'number', hideBelow: 'lg' },
    { key: 'contratos',           header: 'Contratos',           type: 'number', hideBelow: 'lg' },
  ];

  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',            icon: 'fa-solid fa-pen-to-square',       variant: 'edit'   },
    { id: 'lineas', label: 'Líneas de Crédito', icon: 'fa-solid fa-money-check-dollar',  variant: 'info'   },
    { id: 'delete', label: 'Eliminar',          icon: 'fa-solid fa-trash-can',           variant: 'delete',
      disabledFn: (row: FondeadorListItemDto) => (row.lineasCredito ?? 0) > 0 || (row.contratos ?? 0) > 0 },
  ];

  // ── Estado ───────────────────────────────────────────────────
  fondeadores  = signal<FondeadorListItemDto[]>([]);
  loading      = signal(false);
  totalCount   = signal(0);
  totalPages   = signal(0);
  currentPage  = signal(1);
  pageSize     = signal(10);

  query        = { q: '', page: 1, size: 10 };
  searchValue  = '';

  // ── Modal eliminación ─────────────────────────────────────────
  fondeadorAEliminar: FondeadorListItemDto | null = null;

  ngOnInit(): void {
    this.load();
  }

  // ── Eventos GenericTable ──────────────────────────────────────

  onSearch(q: string) {
    this.searchValue = q;
    this.query.q    = q;
    this.query.page = 1;
    this.load();
  }

  onAction(event: TableActionEvent<FondeadorListItemDto>) {
    if (event.action === 'edit')   this.router.navigate(['/configuracion/fondeador/edit', event.row.id]);
    if (event.action === 'lineas') this.router.navigate(
      ['/configuracion/fondeador', event.row.id, 'lineas-credito'],
      { state: { titulo: event.row.titulo } }
    );
    if (event.action === 'delete') this.iniciarEliminacion(event.row);
  }

  onNuevo() {
    this.router.navigate(['/configuracion/fondeador/new']);
  }

  nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.query.page++;
      this.load();
    }
  }

  prevPage() {
    if (this.query.page > 1) {
      this.query.page--;
      this.load();
    }
  }

  // ── Eliminación ───────────────────────────────────────────────

  private iniciarEliminacion(item: FondeadorListItemDto) {
    this.fondeadorAEliminar = item;
    this.confirmModal.show();
  }

  confirmarEliminacion(): void {
    if (!this.fondeadorAEliminar) return;
    this.service.deleteFondeador(this.fondeadorAEliminar.id!).subscribe({
      next: () => {
        this.load();
        this.confirmModal.hide();
        this.fondeadorAEliminar = null;
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? 'Error al eliminar el fondeador.';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      }
    });
  }

  cancelarEliminacion(): void {
    this.fondeadorAEliminar = null;
  }

  // ── Private ──────────────────────────────────────────────────

  private load(): void {
    this.loading.set(true);
    const q = this.query.q || undefined;
    this.service.getPaginatedFondeadores(q, this.query.page, this.query.size).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.fondeadores.set(res.data.results ?? []);
          this.totalCount.set(res.data.totalCount ?? 0);
          this.currentPage.set(res.data.currentPage ?? this.query.page);
          this.pageSize.set(res.data.pageSize ?? this.query.size);
          this.totalPages.set(res.data.totalPages ?? 0);
        } else {
          this.fondeadores.set([]);
          const msg = (res as any).errors?.[0] ?? (res as any).message ?? 'Error al cargar fondeadores';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.fondeadores.set([]);
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar fondeadores', 'error');
        }
      }
    });
  }
}
