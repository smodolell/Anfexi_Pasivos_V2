import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { ContratoPasivoListItem } from 'src/app/core/api/models/contratoPasivoListItem';
import { ContratosAsignadosDto } from 'src/app/core/api/models/contratosAsignadosDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { CardInfoComponent } from '@shared/components/card/card-info.component';

@Component({
  selector: 'app-contratos-pasivos-list',
  standalone: true,
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, CardInfoComponent],
  templateUrl: './contratos-pasivos-list.component.html',
})
export class ContratosPasivosListComponent implements OnInit {
  private readonly selectSvc    = inject(SelectListsService);
  private readonly contratosSvc = inject(ContratosService);
  private readonly utilsService = inject(UtilsService);
  private readonly router       = inject(Router);
  private readonly fb           = inject(FormBuilder);

  // ── Listas de selección ───────────────────────────────────────
  fondeadores        = signal<SelectItemDto[]>([]);
  lineasCredito      = signal<SelectItemDto[]>([]);
  estatusContratos   = signal<SelectItemDto[]>([]);
  loadingFondeadores = signal(false);
  loadingLineas      = signal(false);
  loadingEstatus     = signal(false);

  form = this.fb.group({
    idFondeador:      [null as number | null],
    idLineaCredito:   [null as number | null],
    idEstatusContrato:[null as number | null],
    searchText:       [''],
  });

  // ── Resultados ───────────────────────────────────────────────
  items       = signal<ContratoPasivoListItem[]>([]);
  loading     = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  // ── Contratos asignados ───────────────────────────────────────
  pasivoSeleccionado    = signal<ContratoPasivoListItem | null>(null);
  contratosAsignados    = signal<ContratosAsignadosDto[]>([]);
  loadingAsignados      = signal(false);

  private page           = 1;
  private sortDescending = false;
  sortColumn: string | null = null;
  sortDir: 'asc' | 'desc'  = 'asc';

  private readonly STORAGE_KEY = 'contratos-pasivos-filtros';

  ngOnInit(): void {
    this.cargarFondeadores();
    this.cargarEstatus();
    this.restaurarFiltros();
    this.load();

    this.form.get('idFondeador')!.valueChanges.subscribe(idFondeador => {
      this.form.patchValue({ idLineaCredito: null }, { emitEvent: false });
      this.lineasCredito.set([]);
      if (idFondeador) {
        this.cargarLineasCredito(idFondeador);
      }
    });
  }

  // ── Acciones ─────────────────────────────────────────────────

  onBuscar(): void {
    this.page = 1;
    this.guardarFiltros();
    this.load();
  }

  onLimpiar(): void {
    this.eliminarFiltrosStorage();
    this.form.reset({ idFondeador: null, idLineaCredito: null, idEstatusContrato: null, searchText: '' });
    this.lineasCredito.set([]);
    this.items.set([]);
    this.resetPagination();
  }

  onSort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDir        = this.sortDir === 'asc' ? 'desc' : 'asc';
      this.sortDescending = this.sortDir === 'desc';
    } else {
      this.sortColumn     = column;
      this.sortDir        = 'asc';
      this.sortDescending = false;
    }
    this.page = 1;
    this.load();
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) { this.page++; this.load(); }
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  onEditar(item: ContratoPasivoListItem): void {
    this.router.navigate(['/operaciones/contratos-pasivos/edit', item.id]);
  }

  onVer(item: ContratoPasivoListItem): void {
    this.router.navigate(['/operaciones/contratos-pasivos/view', item.contrato]);
  }

  onActivar(item: ContratoPasivoListItem): void {
    this.router.navigate(
      ['/operaciones/contratos-pasivos/activar', item.id],
      { state: { contrato: item.contrato } },
    );
  }

  onVerAsignados(item: ContratoPasivoListItem): void {
    this.pasivoSeleccionado.set(item);
    this.contratosAsignados.set([]);
    this.loadingAsignados.set(true);

    this.contratosSvc.getContratosAsignados(item.id!).subscribe({
      next: (res) => {
        this.contratosAsignados.set(res.data ?? []);
        this.loadingAsignados.set(false);
      },
      error: (err) => {
        this.loadingAsignados.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar los contratos asignados', 'error');
        }
      },
    });
  }

  onCerrarAsignados(): void {
    this.pasivoSeleccionado.set(null);
    this.contratosAsignados.set([]);
  }

  // ── Private ──────────────────────────────────────────────────

  private load(): void {
    const { idFondeador, idLineaCredito, idEstatusContrato, searchText } = this.form.getRawValue();
    this.loading.set(true);

    this.contratosSvc.getContratosPasivos(
      idFondeador   ?? undefined,
      idEstatusContrato ?? undefined,
      idLineaCredito ?? undefined,
      searchText    || undefined,
      this.page,
      this.pageSize(),
      this.sortColumn ?? undefined,
      this.sortDescending,
    ).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const size  = res.data.pageSize ?? 10;
          const total = res.data.totalCount ?? 0;
          this.items.set(res.data.results ?? []);
          this.currentPage.set(res.data.currentPage ?? this.page);
          this.pageSize.set(size);
          this.totalCount.set(total);
          this.totalPages.set(total > 0 ? Math.ceil(total / size) : 0);
        } else {
          this.resetPagination();
          const msg = res.errors?.[0] ?? res.message ?? 'Error al cargar contratos';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.resetPagination();
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar contratos', 'error');
        }
      },
    });
  }

  private cargarFondeadores(): void {
    this.loadingFondeadores.set(true);
    this.selectSvc.getFondeadoresSelectList().subscribe({
      next: (res) => { this.fondeadores.set(res.data ?? []); this.loadingFondeadores.set(false); },
      error: (err) => {
        this.loadingFondeadores.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar fondeadores', 'error');
        }
      },
    });
  }

  private cargarLineasCredito(idFondeador: number): void {
    this.loadingLineas.set(true);
    this.selectSvc.getLineasCreditoByFondeador(idFondeador).subscribe({
      next: (res) => { this.lineasCredito.set(res.data ?? []); this.loadingLineas.set(false); },
      error: (err) => {
        this.loadingLineas.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar líneas de crédito', 'error');
        }
      },
    });
  }

  private cargarEstatus(): void {
    this.loadingEstatus.set(true);
    this.selectSvc.getEstatusContratoSelectList().subscribe({
      next: (res) => { this.estatusContratos.set(res.data ?? []); this.loadingEstatus.set(false); },
      error: (err) => {
        this.loadingEstatus.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar estatus de contrato', 'error');
        }
      },
    });
  }

  private resetPagination(): void {
    this.items.set([]);
    this.currentPage.set(this.page);
    this.pageSize.set(10);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }

  private guardarFiltros(): void {
    const valores = this.form.getRawValue();
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(valores));
  }

  private eliminarFiltrosStorage(): void {
    localStorage.removeItem(this.STORAGE_KEY);
  }

  private restaurarFiltros(): void {
    const raw = localStorage.getItem(this.STORAGE_KEY);
    if (!raw) return;
    try {
      const filtros = JSON.parse(raw);
      // Patcheamos sin emitir evento para no resetear idLineaCredito por el subscription de idFondeador
      this.form.patchValue(filtros, { emitEvent: false });
      // Si había un fondeador guardado, cargamos sus líneas de crédito manualmente
      if (filtros.idFondeador) {
        this.cargarLineasCredito(filtros.idFondeador);
      }
    } catch {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }
}
