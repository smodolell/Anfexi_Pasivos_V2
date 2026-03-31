import { Component, OnInit, ViewChild, inject, signal } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ConfiguracionesService } from 'src/app/core/api/services/configuraciones.service';
import { LineaCreditoListItemDto } from 'src/app/core/api/models/lineaCreditoListItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { ConfirmModalComponent } from './../../../shared/components/confirm-modal/confirm-modal.component';
import { GenericTableComponent } from '@shared/components/generic-table/generic-table.component';
import { TableColumn, TableAction, TableActionEvent } from '@shared/components/generic-table/table-column.model';

@Component({
  selector: 'app-linea-credito-list',
  standalone: true,
  imports: [RouterModule, GenericTableComponent, ConfirmModalComponent],
  templateUrl: './linea-credito-list.component.html',
})
export class LineaCreditoListComponent implements OnInit {
  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  private readonly service      = inject(ConfiguracionesService);
  private readonly utilsService = inject(UtilsService);
  private readonly router       = inject(Router);
  private readonly route        = inject(ActivatedRoute);

  // ── Tabla ────────────────────────────────────────────────────
  actions: TableAction[] = [
    { id: 'edit',   label: 'Editar',           icon: 'fa-solid fa-pen-to-square', btnClass: 'btn-action-edit'   },
    { id: 'tipos',  label: 'Tipos de Crédito', icon: 'fa-solid fa-tags',          btnClass: 'btn-action-info'   },
    { id: 'delete', label: 'Eliminar',         icon: 'fa-solid fa-trash-can',     btnClass: 'btn-action-delete' },
  ];

  columns: TableColumn[] = [
    { key: 'id',              header: 'ID',               type: 'number',   maxWidth: 70 },
    { key: 'fechaAprobacion', header: 'Fecha Aprobación', type: 'date',     maxWidth: 140 },
    { key: 'montoAprobado',   header: 'Monto Aprobado',   type: 'currency', sortable: true },
    { key: 'montoDispuesto',  header: 'Dispuesto',        type: 'currency', sortable: true, hideBelow: 'md' },
    { key: 'montoDisponible', header: 'Disponible',       type: 'currency', sortable: true, hideBelow: 'md' },
    { key: 'contratos',       header: 'Contratos',        type: 'number',   maxWidth: 100,  hideBelow: 'lg' },
  ];

  // ── Estado ───────────────────────────────────────────────────
  fondeadorTitulo = signal('');
  items           = signal<LineaCreditoListItemDto[]>([]);
  loading         = signal(false);
  totalCount      = signal(0);
  totalPages      = signal(0);
  currentPage     = signal(1);
  pageSize        = signal(10);

  // ── Modal eliminación ─────────────────────────────────────────
  lineaAEliminar: LineaCreditoListItemDto | null = null;

  private idFondeador = 0;
  private q           = '';
  private page        = 1;

  ngOnInit(): void {
    this.idFondeador = +this.route.snapshot.params['id'];
    this.fondeadorTitulo.set(this.router.getCurrentNavigation()?.extras.state?.['titulo'] ?? history.state?.titulo ?? '');
    this.load();
  }

  // ── Eventos tabla ────────────────────────────────────────────

  onSearch(value: string): void {
    this.q    = value;
    this.page = 1;
    this.load();
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) { this.page++; this.load(); }
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  onAction(event: TableActionEvent<LineaCreditoListItemDto>): void {
    if (event.action === 'edit') {
      this.router.navigate(
        ['/configuracion/fondeador', this.idFondeador, 'lineas-credito', 'edit', event.row.id],
        { state: { titulo: this.fondeadorTitulo() } },
      );
    }
    if (event.action === 'tipos') {
      this.router.navigate(
        ['/configuracion/fondeador', this.idFondeador, 'lineas-credito', event.row.id, 'tipos-credito'],
        { state: { titulo: this.fondeadorTitulo() } },
      );
    }
    if (event.action === 'delete') {
      this.lineaAEliminar = event.row;
      this.confirmModal.show();
    }
  }

  confirmarEliminacion(): void {
    if (!this.lineaAEliminar) return;
    this.service.deleteLineaCredito(this.lineaAEliminar.id!).subscribe({
      next: () => {
        this.load();
        this.confirmModal.hide();
        this.lineaAEliminar = null;
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? 'Error al eliminar la línea de crédito.';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  cancelarEliminacion(): void {
    this.lineaAEliminar = null;
  }

  onNuevo(): void {
    this.router.navigate(
      ['/configuracion/fondeador', this.idFondeador, 'lineas-credito', 'new'],
      { state: { titulo: this.fondeadorTitulo() } },
    );
  }

  volver(): void {
    this.router.navigate(['/configuracion/fondeador']);
  }

  // ── Private ──────────────────────────────────────────────────

  private load(): void {
    this.loading.set(true);
    const q = this.q || undefined;

    this.service.getPaginatedLineasCredito(
      q,
      this.idFondeador,
      undefined,
      undefined,
      this.page,
      this.pageSize(),
    ).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.items.set(res.data.results ?? []);
          this.totalCount.set(res.data.totalCount ?? 0);
          this.currentPage.set(res.data.currentPage ?? this.page);
          this.pageSize.set(res.data.pageSize ?? 10);
          this.totalPages.set(res.data.totalPages ?? 0);
        } else {
          this.items.set([]);
          const msg = res.errors?.[0] ?? res.message ?? 'Error al cargar líneas de crédito';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.items.set([]);
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar líneas de crédito', 'error');
        }
      },
    });
  }
}
