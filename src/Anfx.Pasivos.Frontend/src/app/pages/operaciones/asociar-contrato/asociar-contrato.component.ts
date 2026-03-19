import { Component, OnInit, ViewChild, ElementRef, computed, effect, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SelectListsService } from '../../../../api/services/selectLists.service';
import { ContratosService } from '../../../../api/services/contratos.service';
import { SelectItemDto } from '../../../../api/models/selectItemDto';
import { RelActivoPasivoDto } from '../../../../api/models/relActivoPasivoDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';

type RelItem = RelActivoPasivoDto & { seleccionado: boolean };

@Component({
  selector: 'app-asociar-contrato',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './asociar-contrato.component.html',
})
export class AsociarContratoComponent implements OnInit {
  @ViewChild('selectAllCb') selectAllCb!: ElementRef<HTMLInputElement>;
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  private readonly selectSvc    = inject(SelectListsService);
  private readonly contratosSvc = inject(ContratosService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb           = inject(FormBuilder);

  // ── Formulario selección ─────────────────────────────────────
  fondeadores        = signal<SelectItemDto[]>([]);
  contratosPasivos   = signal<SelectItemDto[]>([]);
  loadingFondeadores = signal(false);
  loadingContratos   = signal(false);

  form = this.fb.group({
    idFondeador:      [null as number | null, Validators.required],
    idContratoPasivo: [null as number | null, Validators.required],
  });

  // ── Resultados ───────────────────────────────────────────────
  preparado   = signal(false);
  items       = signal<RelItem[]>([]);
  loading     = signal(false);
  saving      = signal(false);
  totalCount  = signal(0);
  totalPages  = signal(0);
  currentPage = signal(1);
  pageSize    = signal(10);

  private q    = '';
  private page = 1;

  // ── Computed ─────────────────────────────────────────────────
  allSelected        = computed(() => this.items().length > 0 && this.items().every(i => i.seleccionado));
  someSelected       = computed(() => this.items().some(i => i.seleccionado) && !this.allSelected());
  totalSeleccionados = computed(() => this.items().filter(i => i.seleccionado).length);

  constructor() {
    effect(() => {
      const some = this.someSelected();
      if (this.selectAllCb?.nativeElement) {
        this.selectAllCb.nativeElement.indeterminate = some;
      }
    });
  }

  ngOnInit(): void {
    this.cargarFondeadores();

    this.form.get('idFondeador')!.valueChanges.subscribe(idFondeador => {
      this.form.patchValue({ idContratoPasivo: null }, { emitEvent: false });
      this.contratosPasivos.set([]);
      this.preparado.set(false);
      this.items.set([]);
      if (idFondeador) {
        this.cargarContratos(idFondeador);
      }
    });

    this.form.get('idContratoPasivo')!.valueChanges.subscribe(() => {
      this.preparado.set(false);
      this.items.set([]);
    });
  }

  // ── Selección ────────────────────────────────────────────────

  toggleAll(): void {
    const val = !this.allSelected();
    this.items.update(list => list.map(i => ({ ...i, seleccionado: val })));
  }

  toggleItem(item: RelItem): void {
    this.items.update(list =>
      list.map(i => i.id === item.id ? { ...i, seleccionado: !i.seleccionado } : i)
    );
  }

  // ── Acciones ─────────────────────────────────────────────────

  onPreparar(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.preparado.set(true);
    this.q    = '';
    this.page = 1;
    if (this.searchInput?.nativeElement) this.searchInput.nativeElement.value = '';
    this.load();
  }

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

  onGuardar(): void {
    const seleccionados = this.items().filter(i => i.seleccionado).map(i => i.id!);
    if (!seleccionados.length) return;

    const idContratoPasivo = this.form.getRawValue().idContratoPasivo!;
    this.saving.set(true);

    this.contratosSvc.asignarPasivos(idContratoPasivo, { listaContratos: seleccionados }).subscribe({
      next: () => {
        this.utilsService.showNotification('Éxito', 'Contratos activos asignados correctamente', 'success');
        this.saving.set(false);
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al guardar la asignación';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  // ── Private ──────────────────────────────────────────────────

  private load(): void {
    const { idFondeador, idContratoPasivo } = this.form.getRawValue();
    if (!idFondeador || !idContratoPasivo) return;

    this.loading.set(true);
    const q = this.q || undefined;

    this.contratosSvc.getRelActivoPasivo(
      idFondeador,
      idContratoPasivo,
      q,
      this.page,
      this.pageSize(),
    ).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const prevSelected = new Set(this.items().filter(i => i.seleccionado).map(i => i.id));
          this.items.set(
            (res.data.results ?? []).map((r: RelActivoPasivoDto) => ({
              ...r,
              seleccionado: prevSelected.has(r.id),
            }))
          );
          this.totalCount.set(res.data.totalCount ?? 0);
          this.currentPage.set(res.data.currentPage ?? this.page);
          this.pageSize.set(res.data.pageSize ?? 10);
          this.totalPages.set(res.data.totalPages ?? 0);
        } else {
          this.items.set([]);
          const msg = res.errors?.[0] ?? res.message ?? 'Error al cargar datos';
          this.utilsService.showNotification('Error', msg, 'error');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.items.set([]);
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar relaciones', 'error');
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

  private cargarContratos(idFondeador: number): void {
    this.loadingContratos.set(true);
    this.selectSvc.getContratosPasivosPorFondeador(idFondeador).subscribe({
      next: (res) => { this.contratosPasivos.set(res.data ?? []); this.loadingContratos.set(false); },
      error: (err) => {
        this.loadingContratos.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar contratos pasivos', 'error');
        }
      },
    });
  }
}
