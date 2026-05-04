import { Component, computed, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  EMPTY, Subject, Observable, forkJoin, of,
  exhaustMap, switchMap, map, timeout, catchError, tap, filter,
} from 'rxjs';
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

interface ConfigResult {
  config: AnticipoConfigDto | null;
  interes: number | null;
}

@Component({
  selector: 'app-anticipo',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, FormErrorsComponent, ContratoAutocompleteComponent],
  templateUrl: './anticipo.component.html',
})
export class AnticipoComponent {
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  readonly errorSvc = inject(ErrorHandlerService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  tiposTerminacion = signal<SelectItemDto[]>([]);
  tiposReduccion = signal<SelectItemDto[]>([]);
  loadingCatalogos = signal(false);

  contratoBusqueda = signal<string>('');
  buscando = signal(false);
  erroresBusqueda = signal<string[]>([]);

  datosAnticipo = signal<AnticipoDto | null>(null);
  config = signal<AnticipoConfigDto | null>(null);
  guardando = signal(false);
  calculandoConfig = signal(false);
  erroresConfirmacion = signal<string[]>([]);

  private readonly _montoAnticipo = signal<number>(0);
  private readonly _montoInteres = signal<number>(0);

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

  private readonly buscar$ = new Subject<string>();
  private readonly confirmar$ = new Subject<AnticipoDto>();

  form = this.fb.group({
    fechaAnticipo: ['', Validators.required],
    montoAnticipo: [null as number | null, [Validators.required, Validators.min(0.01)]],
    idTipoTerminacion: [null as number | null, Validators.required],
    idTipoReduccion: [{ value: null as number | null, disabled: true }, Validators.required],
    montoInteres: [null as number | null, Validators.required],
  });

  constructor() {
    this.wireCatalogos();
    this.wireMontoChanges();
    this.wireTipoTerminacion();
    this.wireBuscar();
    this.wireConfirmar();
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

  onContratoSelected(item: AutocompleteResultDto): void {
    const contrato = item.label?.trim();
    if (!contrato) return;
    this.contratoBusqueda.set(contrato);
    this.buscar$.next(contrato);
  }

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
    this.confirmar$.next(dto);
  }

  private wireCatalogos(): void {
    this.loadingCatalogos.set(true);
    forkJoin({
      terminaciones: this.selectSvc.getTipoTerminaciones().pipe(timeout(30_000)),
      reducciones: this.selectSvc.getTipoReduccion().pipe(timeout(30_000)),
    }).pipe(
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        this.loadingCatalogos.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar catálogos', 'error');
        }
        return EMPTY;
      }),
    ).subscribe(({ terminaciones, reducciones }) => {
      this.tiposTerminacion.set(terminaciones.data ?? []);
      this.tiposReduccion.set(reducciones.data ?? []);
      this.loadingCatalogos.set(false);
    });
  }

  private wireMontoChanges(): void {
    this.form.get('montoAnticipo')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((v) => this._montoAnticipo.set(v ?? 0));

    this.form.get('montoInteres')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((v) => this._montoInteres.set(v ?? 0));
  }

  private wireTipoTerminacion(): void {
    this.form.get('idTipoTerminacion')!.valueChanges.pipe(
      tap((idTipo) => {
        const ctrlReduccion = this.form.get('idTipoReduccion')!;
        const ctrlAnticipo = this.form.get('montoAnticipo')!;
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
      }),
      filter((idTipo): idTipo is number => !!idTipo && !!this.datosAnticipo()?.idContrato),
      tap(() => this.calculandoConfig.set(true)),
      switchMap((idTipo) => {
        const idContrato = this.datosAnticipo()!.idContrato!;
        return this.buildConfigStream(idTipo, idContrato, null, null).pipe(
          catchError((err: unknown) => {
            this.calculandoConfig.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.erroresConfirmacion.set(this.errorSvc.parseError(err));
            }
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(({ interes }) => {
      if (interes != null) {
        this.form.patchValue({ montoInteres: interes }, { emitEvent: false });
        this._montoInteres.set(interes);
      }
      this.calculandoConfig.set(false);
    });
  }

  private wireBuscar(): void {
    this.buscar$.pipe(
      tap(() => {
        this.buscando.set(true);
        this.erroresBusqueda.set([]);
        this.datosAnticipo.set(null);
        this.config.set(null);
        this._montoAnticipo.set(0);
        this._montoInteres.set(0);
        this.form.reset();
        this.form.get('idTipoReduccion')!.disable({ emitEvent: false });
      }),
      switchMap((contrato) =>
        this.operacionesSvc.getAnticipoByContrato(contrato).pipe(
          timeout(30_000),
          switchMap((res) => {
            const d = res.data!;
            if (d.idTipoTerminacion && d.idContrato) {
              this.calculandoConfig.set(true);
              return this.buildConfigStream(
                d.idTipoTerminacion,
                d.idContrato,
                d.fechaAnticipo?.substring(0, 10) ?? null,
                d.montoAnticipo ?? 0,
              ).pipe(
                map((cfgResult) => ({ anticipo: d, cfgResult })),
                catchError(() => {
                  this.calculandoConfig.set(false);
                  return of({ anticipo: d, cfgResult: null as ConfigResult | null });
                }),
              );
            }
            return of({ anticipo: d, cfgResult: null as ConfigResult | null });
          }),
          catchError((err: unknown) => {
            this.buscando.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.erroresBusqueda.set(this.errorSvc.parseError(err));
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(({ anticipo, cfgResult }) => {
      this.buscando.set(false);
      this.datosAnticipo.set(anticipo);
      this.poblarFormulario(anticipo);
      if (cfgResult?.interes != null) {
        this.form.patchValue({ montoInteres: cfgResult.interes }, { emitEvent: false });
        this._montoInteres.set(cfgResult.interes);
      }
      this.calculandoConfig.set(false);
    });
  }

  private wireConfirmar(): void {
    this.confirmar$.pipe(
      exhaustMap((dto) =>
        this.operacionesSvc.confirmarAnticipo(dto).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.guardando.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.erroresConfirmacion.set(this.errorSvc.parseError(err));
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
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
    });
  }

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
    this._montoAnticipo.set(d.montoAnticipo ?? 0);
    this._montoInteres.set(d.montoInteres ?? 0);
  }

  private buildConfigStream(
    idTipoTerminacion: number,
    idContrato: number,
    overrideFecha: string | null,
    overrideMonto: number | null,
  ): Observable<ConfigResult> {
    return this.operacionesSvc.getAnticipoConfig(idTipoTerminacion, idContrato).pipe(
      timeout(30_000),
      switchMap((res): Observable<ConfigResult> => {
        if (!res.success || !res.data) return of({ config: null, interes: null });

        const cfg = res.data as AnticipoConfigDto;
        this.config.set(cfg);

        const ctrlAnticipo = this.form.get('montoAnticipo')!;
        let monto: number;

        if (cfg.esLiquidacion) {
          monto = cfg.montoAnticipo ?? overrideMonto ?? ((ctrlAnticipo.value as number) ?? 0);
          ctrlAnticipo.setValue(monto, { emitEvent: false });
          ctrlAnticipo.disable({ emitEvent: false });
          this._montoAnticipo.set(monto);
        } else {
          ctrlAnticipo.enable({ emitEvent: false });
          monto = overrideMonto ?? ((ctrlAnticipo.value as number) ?? 0);
        }

        const fecha =
          overrideFecha ?? (this.form.get('fechaAnticipo')!.value as string | null);

        if (cfg.calculaInteres && fecha && monto) {
          return this.operacionesSvc.getInteres(idContrato, fecha, monto).pipe(
            timeout(30_000),
            map(
              (interesRes): ConfigResult => ({
                config: cfg,
                interes:
                  interesRes.success && interesRes.data != null
                    ? (interesRes.data as number)
                    : null,
              }),
            ),
          );
        }

        return of({ config: cfg, interes: null });
      }),
    );
  }
}
