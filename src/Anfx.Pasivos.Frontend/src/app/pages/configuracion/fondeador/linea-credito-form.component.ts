import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ConfiguracionesService } from 'src/app/core/api/services/configuraciones.service';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { LineaCreditoEditDto } from 'src/app/core/api/models/lineaCreditoEditDto';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';

@Component({
  selector: 'app-linea-credito-form',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule],
  templateUrl: './linea-credito-form.component.html',
})
export class LineaCreditoFormComponent implements OnInit {
  private readonly service      = inject(ConfiguracionesService);
  private readonly selectSvc    = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  private readonly router       = inject(Router);
  private readonly route        = inject(ActivatedRoute);
  private readonly fb           = inject(FormBuilder);

  isEditMode      = signal(false);
  isLoading       = signal(false);
  errorMsg        = signal<string | null>(null);
  monedas         = signal<SelectItemDto[]>([]);
  tasas           = signal<SelectItemDto[]>([]);
  fondeadorTitulo = signal('');

  private idFondeador = 0;
  private lineaId     = 0;

  form = this.fb.group({
    // ── Solo lectura ──────────────────────────────────────────
    idFondeador:              [{ value: '', disabled: true }],
    montoDispuesto:           [{ value: null as number | null, disabled: true }],
    montoDisponible:          [{ value: null as number | null, disabled: true }],
    fechaUltimaDisposicion:   [{ value: null as string | null, disabled: true }],
    fechaAmpliacion:          [{ value: null as string | null, disabled: true }],
    montoRevolvente:          [{ value: null as number | null, disabled: true }],
    noDisposiciones:          [{ value: null as number | null, disabled: true }],
    // ── Editables ─────────────────────────────────────────────
    idMoneda:                 [null as number | null, Validators.required],
    fechaAprobacion:          ['', Validators.required],
    plazoMaximo:              [null as number | null],
    montoAprobado:            [null as number | null, [Validators.required, Validators.min(0.01)]],
    esRevolvente:             [false],
    fechaMaxDisposicion:      [null as string | null],
    tipoTasa:                 [false],
    idTasa:                   [null as number | null, Validators.required],
    tasa:                     [null as number | null, [Validators.required, Validators.min(0)]],
    activo:                   [true],
  });

  ngOnInit(): void {
    this.idFondeador = +this.route.snapshot.params['id'];
    this.lineaId     = +this.route.snapshot.params['lineaId'] || 0;
    this.fondeadorTitulo.set(history.state?.titulo ?? '');
    this.isEditMode.set(this.lineaId > 0);

    // Pre-rellenar con el nombre del state; se sobreescribe con la API en loadCatalogos
    this.form.get('idFondeador')!.setValue(this.fondeadorTitulo() || '');

    // Cuando el usuario cambia tipoTasa: recargar tasas y limpiar idTasa / tasa
    this.form.get('tipoTasa')!.valueChanges.subscribe(esVariable => {
      this.form.patchValue({ idTasa: null, tasa: null }, { emitEvent: false });
      this.cargarTasas(esVariable ?? false);
    });

    // Cuando se selecciona una tasa: copiar su valueDecimal al campo tasa
    this.form.get('idTasa')!.valueChanges.subscribe(idTasa => {
      const item = this.tasas().find(t => t.value === Number(idTasa));
          if (item?.valueDecimal !== undefined) {
        this.form.patchValue({ tasa: item.valueDecimal }, { emitEvent: false });
      }
    });

    this.loadCatalogos();
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMsg.set(null);
    const v = this.form.getRawValue();

    // Solo se envían los campos editables; los readonly son calculados por el servidor
    const dto: LineaCreditoEditDto = {
      idFondeador:         this.idFondeador,
      fondeador:           this.fondeadorTitulo(),
      idMoneda:            v.idMoneda!,
      montoAprobado:       v.montoAprobado!,
      fechaAprobacion:     v.fechaAprobacion!,
      tipoTasa:            v.tipoTasa ?? false,
      idTasa:              v.idTasa!,
      tasa:                v.tasa!,
      esRevolvente:        v.esRevolvente ?? false,
      plazoMaximo:         v.plazoMaximo ?? undefined,
      fechaMaxDisposicion: v.fechaMaxDisposicion ?? null,
      activo:              v.activo ?? true,
    };

    const request$ = this.isEditMode()
      ? this.service.updateLineaCredito(this.lineaId, dto)
      : this.service.createLineaCredito(dto);

    request$.subscribe({
      next: () => this.volverALista(),
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.errorMsg.set(err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al procesar la solicitud.');
        }
      },
    });
  }

  volverALista(): void {
    this.router.navigate(
      ['/configuracion/fondeador', this.idFondeador, 'lineas-credito'],
      { state: { titulo: this.fondeadorTitulo() } },
    );
  }

  // ── Private ──────────────────────────────────────────────────

  private cargarTasas(esVariable: boolean): void {
    this.selectSvc.getTasas(esVariable).subscribe({
      next: (res) => this.tasas.set(res.data ?? []),
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar tasas', 'error');
        }
      },
    });
  }

  private loadCatalogos(): void {
    this.isLoading.set(true);

    if (this.isEditMode()) {
      forkJoin({
        monedas:   this.selectSvc.getMonedas(),
        fondeador: this.service.getFondeadorById(this.idFondeador),
        linea:     this.service.getLineaCreditoById(this.lineaId),
      }).subscribe({
        next: ({ monedas, fondeador, linea }) => {
          this.monedas.set(monedas.data ?? []);

          const nombreFondeador = fondeador.data?.fondeador ?? '';
          this.fondeadorTitulo.set(nombreFondeador);
          this.form.get('idFondeador')!.setValue(nombreFondeador);

          if (linea.success && linea.data) {
            const d          = linea.data;
            const esVariable = d.tipoTasa ?? false;

            this.selectSvc.getTasas(esVariable).subscribe({
              next: (tasasRes) => {
                this.tasas.set(tasasRes.data ?? []);

                this.form.patchValue({
                  idMoneda:               d.idMoneda                                    ?? null,
                  montoAprobado:          d.montoAprobado                               ?? null,
                  montoDispuesto:         d.montoDispuesto                              ?? null,
                  montoDisponible:        d.montoDisponible                             ?? null,
                  montoRevolvente:        d.montoRevolvente                             ?? null,
                  fechaAprobacion:        d.fechaAprobacion?.substring(0, 10)           ?? '',
                  fechaUltimaDisposicion: d.fechaUltimaDisposicion?.substring(0, 10)   ?? null,
                  fechaAmpliacion:        d.fechaAmpliacion?.substring(0, 10)           ?? null,
                  fechaMaxDisposicion:    d.fechaMaxDisposicion?.substring(0, 10)       ?? null,
                  noDisposiciones:        d.noDisposiciones                             ?? null,
                  idTasa:                 d.idTasa                                      ?? null,
                  tasa:                   d.tasa                                        ?? null,
                  esRevolvente:           d.esRevolvente                                ?? false,
                  plazoMaximo:            d.plazoMaximo                                 ?? null,
                  activo:                 d.activo                                      ?? true,
                });
                // Setear tipoTasa sin disparar valueChanges para no resetear idTasa
                this.form.get('tipoTasa')!.setValue(esVariable, { emitEvent: false });

                this.isLoading.set(false);
              },
              error: (err) => {
                this.isLoading.set(false);
                if (!wasHandledByInterceptor(err)) this.errorMsg.set('Error al cargar catálogo de tasas.');
              },
            });
          } else {
            this.errorMsg.set(linea.errors?.[0] ?? 'Error al cargar la línea de crédito.');
            this.isLoading.set(false);
          }
        },
        error: (err) => {
          this.isLoading.set(false);
          if (!wasHandledByInterceptor(err)) this.errorMsg.set('Error de conexión al cargar datos.');
        },
      });
    } else {
      forkJoin({
        monedas:   this.selectSvc.getMonedas(),
        tasas:     this.selectSvc.getTasas(false),
        fondeador: this.service.getFondeadorById(this.idFondeador),
      }).subscribe({
        next: ({ monedas, tasas, fondeador }) => {
          this.monedas.set(monedas.data ?? []);
          this.tasas.set(tasas.data ?? []);

          const nombreFondeador = fondeador.data?.fondeador ?? '';
          this.fondeadorTitulo.set(nombreFondeador);
          this.form.get('idFondeador')!.setValue(nombreFondeador);

          this.isLoading.set(false);
        },
        error: (err) => {
          this.isLoading.set(false);
          if (!wasHandledByInterceptor(err)) this.errorMsg.set('Error de conexión al cargar catálogos.');
        },
      });
    }
  }
}
