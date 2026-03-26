import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ContratosService } from '../../../../api/services/contratos.service';
import { SelectListsService } from '../../../../api/services/selectLists.service';
import { ConfiguracionesService } from '../../../../api/services/configuraciones.service';
import { ContratoPasivoDto } from '../../../../api/models/contratoPasivoDto';
import { SelectItemDto } from '../../../../api/models/selectItemDto';
import { TipoTablaAmortizaListItemDto } from '../../../../api/models/tipoTablaAmortizaListItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-contrato-view',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, RouterModule],
  templateUrl: './contrato-view.component.html',
})
export class ContratoViewComponent implements OnInit {
  private readonly contratosSvc       = inject(ContratosService);
  private readonly selectSvc          = inject(SelectListsService);
  private readonly configuracionesSvc = inject(ConfiguracionesService);
  private readonly utilsSvc           = inject(UtilsService);
  private readonly router             = inject(Router);
  private readonly route              = inject(ActivatedRoute);

  isLoading = signal(true);
  contrato  = signal<ContratoPasivoDto | null>(null);

  // ── Listas ───────────────────────────────────────────────────
  estatusContratoList          = signal<SelectItemDto[]>([]);
  monedas                      = signal<SelectItemDto[]>([]);
  periodicidades               = signal<SelectItemDto[]>([]);
  tiposTabla                   = signal<TipoTablaAmortizaListItemDto[]>([]);
  tipoCapitalizacionList       = signal<SelectItemDto[]>([]);
  tipoPagoCapitalList          = signal<SelectItemDto[]>([]);
  periodicidadesCapitalizacion = signal<SelectItemDto[]>([]);
  tasasOrdinarias              = signal<SelectItemDto[]>([]);
  tasasMora                    = signal<SelectItemDto[]>([]);

  // ── Textos resueltos ─────────────────────────────────────────
  estatusText      = computed(() => this.resolve(this.estatusContratoList(), this.contrato()?.idEstatusContrato));
  monedaText       = computed(() => this.resolve(this.monedas(),             this.contrato()?.idMoneda));
  periodicidadText = computed(() => this.resolve(this.periodicidades(),      this.contrato()?.idPeriodicidad));
  tasaText         = computed(() => this.resolve(this.tasasOrdinarias(),     this.contrato()?.idTasa));
  tasaMoraText     = computed(() => this.resolve(this.tasasMora(),           this.contrato()?.idTasaMora));
  tipoCapText      = computed(() => this.resolve(this.tipoCapitalizacionList(),  this.contrato()?.idTipoCapitalizacion));
  tipoPagoText     = computed(() => this.resolve(this.tipoPagoCapitalList(),     this.contrato()?.idTipoPagoCapital));
  periodicidadTTAText = computed(() => this.resolve(this.periodicidadesCapitalizacion(), this.contrato()?.idPeriodicidad_TTA));

  tablaText = computed(() => {
    const id = this.contrato()?.idTipoTablaAmortiza;
    if (!id) return '—';
    return this.tiposTabla().find(t => t.id === id)?.tipoTablaAmortiza ?? '—';
  });

  tipoTasaText = computed(() => {
    const v = this.contrato()?.tipoTasa;
    return v === null || v === undefined ? '—' : (v ? 'Variable' : 'Fija');
  });

  tipoTasaMoraText = computed(() => {
    const v = this.contrato()?.tipoTasaMora;
    return v === null || v === undefined ? '—' : (v ? 'Variable' : 'Fija');
  });

  // ── Lifecycle ────────────────────────────────────────────────

  ngOnInit(): void {
    const id = +this.route.snapshot.params['id'];

    forkJoin({
      estatusContrato: this.selectSvc.getEstatusContratoSelectList(),
      monedas:         this.selectSvc.getMonedas(),
      periodicidades:  this.selectSvc.getPeriodicidadSelectList(),
      tiposTabla:      this.configuracionesSvc.apiConfiguracionesTipoTablaAmortizaGet(undefined, 1, 100),
    }).subscribe({
      next: ({ estatusContrato, monedas, periodicidades, tiposTabla }) => {
        this.estatusContratoList.set(estatusContrato.data ?? []);
        this.monedas.set(monedas.data ?? []);
        this.periodicidades.set(periodicidades.data ?? []);
        this.tiposTabla.set(tiposTabla.data?.results ?? []);
        this.cargarContrato(id);
      },
      error: () => this.cargarContrato(id),
    });
  }

  onVolver(): void {
    this.router.navigate(['/operaciones/contratos-pasivos']);
  }

  // ── Private ──────────────────────────────────────────────────

  private cargarContrato(id: number): void {
    this.contratosSvc.getContratoById(id).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          const d = res.data;
          this.contrato.set(d);

          if (d.tipoTasa !== null && d.tipoTasa !== undefined) {
            this.selectSvc.getTasas(d.tipoTasa === true).subscribe(r => this.tasasOrdinarias.set(r.data ?? []));
          }
          if (d.tipoTasaMora !== null && d.tipoTasaMora !== undefined) {
            this.selectSvc.getTasas(d.tipoTasaMora === true).subscribe(r => this.tasasMora.set(r.data ?? []));
          }

          if (d.idTipoTablaAmortiza) {
            const period$ = d.idTipoTablaAmortiza === 1
              ? this.selectSvc.getPeriodicidadSelectList()
              : this.selectSvc.getPeriodicidadTTASelectList(d.idTipoTablaAmortiza);

            forkJoin({ info: this.contratosSvc.getTipoTablaAmortizaInfo(d.idTipoTablaAmortiza), period: period$ })
              .subscribe(({ info, period }) => {
                if (info.success && info.data) {
                  this.tipoCapitalizacionList.set(info.data.tipoCapitalizacion ?? []);
                  this.tipoPagoCapitalList.set(info.data.tipoPagoCapital ?? []);
                }
                this.periodicidadesCapitalizacion.set(period.data ?? []);
              });
          }
        } else {
          this.utilsSvc.showNotification('Error', 'No se pudo cargar el contrato', 'error');
          this.router.navigate(['/operaciones/contratos-pasivos']);
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar el contrato', 'error');
        }
        this.router.navigate(['/operaciones/contratos-pasivos']);
      },
    });
  }

  private resolve(list: SelectItemDto[], id?: number | null): string {
    if (id == null) return '—';
    return list.find(i => i.value === id)?.text ?? '—';
  }

  fmt(val?: string | null): string {
    if (!val) return '—';
    return val.substring(0, 10);
  }
}
