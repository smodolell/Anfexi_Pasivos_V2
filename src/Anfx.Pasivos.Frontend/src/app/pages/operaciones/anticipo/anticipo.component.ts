import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, of, switchMap } from 'rxjs';
import { AnticipoConfigDto } from 'src/app/core/api/models/anticipoConfigDto';
import { AnticipoDto } from 'src/app/core/api/models/anticipoDto';
import { AutocompleteResultDto } from 'src/app/core/api/models/autocompleteResultDto';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { OperacionesService } from 'src/app/core/api/services/operaciones.service';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { ErrorHandlerService } from '@services/error.services';
import { UtilsService } from '@services/utils.service';
import { ContratoAutocompleteComponent } from '@shared/components/contrato-autocomplete/contrato-autocomplete.component';
import { FormErrorsComponent } from 'src/app/shared/components/form-errors/form-error.component';

@Component({
  selector: 'app-anticipo',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, FormErrorsComponent, ContratoAutocompleteComponent],
  templateUrl: './anticipo.component.html',
})
export class AnticipoComponent implements OnInit {
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  readonly errorSvc = inject(ErrorHandlerService);
  private readonly fb = inject(FormBuilder);

  // ── Catálogos ────────────────────────────────────────────────
  tiposTerminacion = signal<SelectItemDto[]>([]);
  tiposReduccion = signal<SelectItemDto[]>([]);
  loadingCatalogos = signal(false);

  // ── Estado búsqueda ──────────────────────────────────────────
  contratoBusqueda = signal<string>('');
  buscando = signal(false);
  erroresBusqueda = signal<string[]>([]);

  // ── Datos cargados ───────────────────────────────────────────
  datosAnticipo = signal<AnticipoDto | null>(null);
  config = signal<AnticipoConfigDto | null>(null);
  guardando = signal(false);
  calculandoConfig = signal(false);
  erroresConfirmacion = signal<string[]>([]);

  // ── Valores base para cómputo (sincronizados desde el form) ──
  private readonly _montoAnticipo = signal<number>(0);
  private readonly _montoInteres = signal<number>(0);

  // ── Computed signals ─────────────────────────────────────────
  readonly montoIVA_Interes = computed(
    () => this._montoInteres() * (this.config()?.porcIVA_Interes ?? 0),
  );
  readonly montoPena = computed(() => this.datosAnticipo()?.montoPena ?? 0);
  readonly montoIVA_Pena = computed(
    () => this.montoPena() * (this.config()?.porcIVA_Pena ?? 0),
  );
  readonly montoTotal = computed(
    () =>
      this._montoAnticipo() +
      this._montoInteres() +
      this.montoIVA_Interes() +
      this.montoPena() +
      this.montoIVA_Pena(),
  );

  // ── Formulario ───────────────────────────────────────────────
  form = this.fb.group({
    fechaAnticipo: ['', Validators.required],
    montoAnticipo: [null as number | null, [Validators.required, Validators.min(0.01)]],
    idTipoTerminacion: [null as number | null, Validators.required],
    idTipoReduccion: [{ value: null as number | null, disabled: true }, Validators.required],
    montoInteres: [null as number | null, Validators.required],
  });

  constructor() {
    // Sincronizar form → signals para el cómputo del total
    this.form.get('montoAnticipo')!.valueChanges.subscribe((v) =>
      this._montoAnticipo.set(v ?? 0),
    );
    this.form.get('montoInteres')!.valueChanges.subscribe((v) =>
      this._montoInteres.set(v ?? 0),
    );

    // Al cambiar el tipo: resetear estado y recargar configuración
    this.form.get('idTipoTerminacion')!.valueChanges.subscribe((idTipo) => {
      const ctrlReduccion = this.form.get('idTipoReduccion')!;
      const ctrlAnticipo = this.form.get('montoAnticipo')!;

      // Resetear config y montos calculados
      this.config.set(null);
      this._montoInteres.set(0);
      this.form.patchValue({ montoInteres: null }, { emitEvent: false });
      ctrlAnticipo.enable({ emitEvent: false });

      if (idTipo === 1) {
        ctrlReduccion.enable({ emitEvent: false });
      } else {
        ctrlReduccion.disable({ emitEvent: false });
        ctrlReduccion.setValue(null, { emitEvent: false });
      }

      if (idTipo && this.datosAnticipo()?.idContrato) {
        this.cargarConfig(idTipo, this.datosAnticipo()!.idContrato!);
      }
    });
  }

  ngOnInit(): void {
    this.loadingCatalogos.set(true);
    forkJoin({
      terminaciones: this.selectSvc.getTipoTerminaciones(),
      reducciones: this.selectSvc.getTipoReduccion(),
    }).subscribe({
      next: ({ terminaciones, reducciones }) => {
        this.tiposTerminacion.set(terminaciones.data ?? []);
        this.tiposReduccion.set(reducciones.data ?? []);
        this.loadingCatalogos.set(false);
      },
      error: (err: unknown) => {
        this.loadingCatalogos.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar catálogos', 'error');
        }
      },
    });
  }

  get mostrarFormulario(): boolean {
    return this.datosAnticipo() !== null;
  }

  get reduccionHabilitada(): boolean {
    return this.form.get('idTipoReduccion')!.enabled;
  }

  get esLiquidacion(): boolean {
    return this.config()?.esLiquidacion ?? false;
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  // ── Búsqueda ─────────────────────────────────────────────────
  onContratoSelected(item: AutocompleteResultDto): void {
    const contrato = item.label?.trim();
    if (!contrato) return;

    this.contratoBusqueda.set(contrato);
    this.buscando.set(true);
    this.erroresBusqueda.set([]);
    this.datosAnticipo.set(null);
    this.config.set(null);
    this._montoAnticipo.set(0);
    this._montoInteres.set(0);
    this.form.reset();
    this.form.get('idTipoReduccion')!.disable({ emitEvent: false });

    this.operacionesSvc.getAnticipoByContrato(contrato).subscribe({
      next: (res) => {
        this.buscando.set(false);
        this.datosAnticipo.set(res.data!);
        this.poblarFormulario(res.data!);
      },
      error: (err: unknown) => {
        this.buscando.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.erroresBusqueda.set(this.errorSvc.parseError(err));
        }
      },
      complete: () => this.buscando.set(false),
    });
  }

  // ── Confirmar ────────────────────────────────────────────────
  onConfirmar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const base = this.datosAnticipo()!;

    const dto: AnticipoDto = {
      idContrato: base.idContrato,
      fechaAnticipo: v.fechaAnticipo!,
      montoAnticipo: v.montoAnticipo!,
      idTipoTerminacion: v.idTipoTerminacion!,
      idTipoReduccion: v.idTipoReduccion ?? 0,
      montoInteres: v.montoInteres!,
      montoIVA_Interes: this.montoIVA_Interes(),
      montoPena: this.montoPena(),
      montoIVA_Pena: this.montoIVA_Pena(),
      montoTotal: this.montoTotal(),
      esLiquidacion: this.esLiquidacion,
    };

    this.guardando.set(true);
    this.erroresConfirmacion.set([]);
    this.operacionesSvc.confirmarAnticipo(dto).subscribe({
      next: (res) => {
        this.guardando.set(false);
        const msg = res?.message ?? 'Anticipo procesado correctamente';
        this.utilsService.showNotification('Éxito', msg, 'success');
        this.datosAnticipo.set(null);
        this.contratoBusqueda.set('');
        this.config.set(null);
        this._montoAnticipo.set(0);
        this._montoInteres.set(0);
        this.form.reset();
        this.form.get('idTipoReduccion')!.disable({ emitEvent: false });
      },
      error: (err: unknown) => {
        this.guardando.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.erroresConfirmacion.set(this.errorSvc.parseError(err));
        }
      },
      complete: () => this.guardando.set(false),
    });
  }

  // ── Private ──────────────────────────────────────────────────
  private poblarFormulario(d: AnticipoDto): void {
    if ((d.idTipoTerminacion ?? 0) === 1) {
      this.form.get('idTipoReduccion')!.enable({ emitEvent: false });
    }

    this.form.patchValue(
      {
        fechaAnticipo: d.fechaAnticipo?.substring(0, 10) ?? '',
        montoAnticipo: d.montoAnticipo ?? null,
        idTipoTerminacion: d.idTipoTerminacion ?? null,
        idTipoReduccion: d.idTipoReduccion ?? null,
        montoInteres: d.montoInteres ?? 0,
      },
      { emitEvent: false },
    );
    // Sincronizar signals manualmente (emitEvent:false suprime valueChanges)
    this._montoAnticipo.set(d.montoAnticipo ?? 0);
    this._montoInteres.set(d.montoInteres ?? 0);

    if (d.idTipoTerminacion && d.idContrato) {
      this.cargarConfig(d.idTipoTerminacion, d.idContrato);
    }
  }

  private cargarConfig(idTipoTerminacion: number, idContrato: number): void {
    this.calculandoConfig.set(true);

    this.operacionesSvc
      .getAnticipoConfig(idTipoTerminacion, idContrato)
      .pipe(
        switchMap((res) => {
          if (!res.success || !res.data) return of({ config: null, interes: null });

          const cfg = res.data as AnticipoConfigDto;
          this.config.set(cfg);

          const ctrlAnticipo = this.form.get('montoAnticipo')!;

          if (cfg.esLiquidacion) {
            // Liquidación: precargar monto fijo y deshabilitar el campo
            const monto = cfg.montoAnticipo ?? (ctrlAnticipo.value as number) ?? 0;
            ctrlAnticipo.setValue(monto, { emitEvent: false });
            ctrlAnticipo.disable({ emitEvent: false });
            this._montoAnticipo.set(monto);
          } else {
            ctrlAnticipo.enable({ emitEvent: false });
          }

          if (cfg.calculaInteres) {
            const fechaAnticipo = this.form.get('fechaAnticipo')!.value as string;
            const montoAnticipo = cfg.esLiquidacion
              ? (cfg.montoAnticipo ?? (ctrlAnticipo.value as number) ?? 0)
              : ((ctrlAnticipo.value as number) ?? 0);

            if (fechaAnticipo && montoAnticipo) {
              return this.operacionesSvc
                .getInteres(idContrato, fechaAnticipo, montoAnticipo)
                .pipe(switchMap((interesRes) => of({ config: cfg, interes: interesRes })));
            }
          }

          return of({ config: cfg, interes: null });
        }),
      )
      .subscribe({
        next: ({ config: cfg, interes }) => {
          console.log(cfg);
          console.log(interes);
          if (cfg && interes?.success && interes.data != null) {
            const montoInteres = interes.data as number;
            this.form.patchValue({ montoInteres }, { emitEvent: false });
            this._montoInteres.set(montoInteres);
          }
          this.calculandoConfig.set(false);
        },
        error: (err: unknown) => {
          this.calculandoConfig.set(false);
          if (!wasHandledByInterceptor(err)) {
            this.erroresConfirmacion.set(this.errorSvc.parseError(err));
          }
        },
      });
  }
}
