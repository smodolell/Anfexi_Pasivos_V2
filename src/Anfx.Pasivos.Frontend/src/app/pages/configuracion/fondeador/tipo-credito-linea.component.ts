import { Component, OnInit, ViewChild, ElementRef, computed, effect, inject, signal } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ConfiguracionesService } from 'src/app/core/api/services/configuraciones.service';
import { RelLineaCreditoTipoCreditoDto } from 'src/app/core/api/models/relLineaCreditoTipoCreditoDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';

@Component({
  selector: 'app-tipo-credito-linea',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './tipo-credito-linea.component.html',
})
export class TipoCreditoLineaComponent implements OnInit {
  @ViewChild('selectAllCb') selectAllCb!: ElementRef<HTMLInputElement>;

  private readonly service      = inject(ConfiguracionesService);
  private readonly utilsService = inject(UtilsService);
  private readonly router       = inject(Router);
  private readonly route        = inject(ActivatedRoute);

  // ── Estado ───────────────────────────────────────────────────
  items       = signal<RelLineaCreditoTipoCreditoDto[]>([]);
  loading     = signal(false);
  saving      = signal(false);
  errorMsg    = signal<string | null>(null);
  fondeadorTitulo = signal('');

  private idFondeador = 0;
  private lineaId     = 0;

  // ── Computed ─────────────────────────────────────────────────
  allSelected  = computed(() => this.items().length > 0 && this.items().every(i => i.seleccionado));
  someSelected = computed(() => this.items().some(i => i.seleccionado) && !this.allSelected());
  totalSeleccionados = computed(() => this.items().filter(i => i.seleccionado).length);

  constructor() {
    // Sincroniza el estado indeterminate del checkbox "Seleccionar todos"
    effect(() => {
      const some = this.someSelected();
      if (this.selectAllCb?.nativeElement) {
        this.selectAllCb.nativeElement.indeterminate = some;
      }
    });
  }

  ngOnInit(): void {
    this.idFondeador = +this.route.snapshot.params['id'];
    this.lineaId     = +this.route.snapshot.params['lineaId'];
    this.fondeadorTitulo.set(history.state?.titulo ?? '');
    this.load();
  }

  // ── Selección ────────────────────────────────────────────────

  toggleAll(): void {
    const selectAll = !this.allSelected();
    this.items.update(list => list.map(i => ({ ...i, seleccionado: selectAll })));
  }

  toggleItem(item: RelLineaCreditoTipoCreditoDto): void {
    this.items.update(list =>
      list.map(i => i.idTipoCredito === item.idTipoCredito
        ? { ...i, seleccionado: !i.seleccionado }
        : i
      )
    );
  }

  // ── Guardar ──────────────────────────────────────────────────

  onGuardar(): void {
    this.saving.set(true);
    this.service.saveTiposCreditoByLineaCredito(this.lineaId, this.items()).subscribe({
      next: () => {
        this.utilsService.showNotification('Éxito', 'Tipos de crédito guardados correctamente', 'success');
        this.saving.set(false);
      },
      error: (err) => {
        this.saving.set(false);
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al guardar';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  volver(): void {
    this.router.navigate(
      ['/configuracion/fondeador', this.idFondeador, 'lineas-credito'],
      { state: { titulo: this.fondeadorTitulo() } },
    );
  }

  // ── Private ──────────────────────────────────────────────────

  private load(): void {
    this.loading.set(true);
    this.errorMsg.set(null);

    this.service.getTiposCreditoByLineaCredito(this.lineaId).subscribe({
      next: (res) => {
        if (res.success) {
          this.items.set(res.data ?? []);
        } else {
          this.errorMsg.set(res.errors?.[0] ?? res.message ?? 'Error al cargar tipos de crédito');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.errorMsg.set('Error de conexión al cargar tipos de crédito');
        }
      },
    });
  }
}
