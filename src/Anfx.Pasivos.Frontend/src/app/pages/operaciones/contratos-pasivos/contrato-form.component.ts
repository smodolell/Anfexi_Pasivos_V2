import { Component, OnInit, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  EMPTY, Subject, forkJoin,
  exhaustMap, switchMap, timeout, catchError, tap, filter,
} from 'rxjs';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { ConfiguracionesService } from 'src/app/core/api/services/configuraciones.service';
import { ContratoPasivoEditDto } from 'src/app/core/api/models/contratoPasivoEditDto';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { TablaAmortizaItemDto } from 'src/app/core/api/models/tablaAmortizaItemDto';
import { TipoTablaAmortizaListItemDto } from 'src/app/core/api/models/tipoTablaAmortizaListItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { FormErrorsComponent } from '@shared/components/form-errors/form-error.component';
import { ErrorHandlerService } from '@services/error.services';

interface PagoFormValue {
  noPago: number | null;
  capital: number | null;
  fecVencimiento: string | null;
}

@Component({
  selector: 'app-contrato-form',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, RouterModule, ReactiveFormsModule, FormErrorsComponent],
  templateUrl: './contrato-form.component.html',
})
export class ContratoFormComponent implements OnInit {
  private readonly contratosSvc = inject(ContratosService);
  private readonly selectSvc = inject(SelectListsService);
  private readonly configuracionesSvc = inject(ConfiguracionesService);
  private readonly utilsSvc = inject(UtilsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  readonly errorSvc = inject(ErrorHandlerService);

  isEditMode = signal(false);
  contratoId = signal<number | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);
  loadingClave = signal(false);

  tiposCredito = signal<SelectItemDto[]>([]);
  monedas = signal<SelectItemDto[]>([]);
  periodicidades = signal<SelectItemDto[]>([]);
  tasasIva = signal<SelectItemDto[]>([]);
  tasasOrdinarias = signal<SelectItemDto[]>([]);
  tasasMora = signal<SelectItemDto[]>([]);
  tiposTabla = signal<TipoTablaAmortizaListItemDto[]>([]);
  tipoCapitalizacionList = signal<SelectItemDto[]>([]);
  tipoPagoCapitalList = signal<SelectItemDto[]>([]);
  periodicidadesCapitalizacion = signal<SelectItemDto[]>([]);
  loadingTablaInfo = signal(false);
  esCapitalizable = signal(false);
  formErrors = signal<string[]>([]);
  tablaAmortizacion = signal<TablaAmortizaItemDto[]>([]);
  loadingTablaAmortizacion = signal(false);
  estatusContratoList = signal<SelectItemDto[]>([]);

  private readonly fieldLabels: Record<string, string> = {
    idTipoCredito: 'Tipo de Crédito',
    contrato: 'Clave de Contrato',
    capitalFinanciado: 'Capital Financiado',
    fecInicioContrato: 'Fecha de Alta de Contrato',
  };

  readonly tipoTasaOpciones = [
    { value: false, text: 'Fija' },
    { value: true, text: 'Variable' },
  ];

  form = this.fb.group({
    fondeador: [{ value: '', disabled: true }],
    lineaCreditoNombre: [{ value: '', disabled: true }],
    maxCapitalDisponible: [{ value: null as number | null, disabled: true }],

    idTipoCredito: [null as number | null, Validators.required],
    contrato: ['', Validators.required],
    estatusContrato: [{ value: '', disabled: true }],
    idEstatusContrato: [{ value: null as number | null, disabled: true }],

    capitalFinanciado: [null as number | null, [Validators.required, Validators.min(0.01)]],
    plazo: [null as number | null],
    idMoneda: [{ value: null as number | null, disabled: true }],
    idPeriodicidad: [{ value: null as number | null, disabled: true }],

    tipoTasa: [null as boolean | null],
    idTasa: [null as number | null],
    tasaIva: [null as number | null],
    tasa: [null as number | null],
    tasaBase: [{ value: null as number | null, disabled: true }],
    puntosMas: [null as number | null],
    puntosPor: [null as number | null],

    tipoTasaMora: [null as boolean | null],
    idTasaMora: [null as number | null],
    tasaMora: [null as number | null],
    tasaBaseMora: [{ value: null as number | null, disabled: true }],
    puntosMasMora: [null as number | null],
    factorMora: [null as number | null],

    fecInicioContrato: ['', Validators.required],
    fechaFirmaContrato: [null as string | null],
    fecPrimeraRenta: [null as string | null],
    fecActivacion: [{ value: null as string | null, disabled: true }],
    fecFinContrato: [{ value: null as string | null, disabled: true }],

    idTipoTablaAmortiza: [null as number | null],
    idTipoCapitalizacion: [{ value: null as number | null, disabled: true }],
    idTipoPagoCapital: [{ value: null as number | null, disabled: true }],
    idPeriodicidad_TTA: [{ value: null as number | null, disabled: true }],
    noPagosIrregulares: [{ value: null as number | null, disabled: true }],

    idLineaCredito: [{ value: null as number | null, disabled: true }],
    idFondeador: [{ value: null as number | null, disabled: true }],
    tasaEsVariable: [null as boolean | null],
    versionTabla: [null as number | null],
    idTipoCalculoTasaVariable: [{ value: null as number | null, disabled: true }],
    idTipoMantenimiento: [{ value: null as number | null, disabled: true }],
    nroRentasDepositoGarantia: [null as number | null],
    factorFIRA: [null as number | null],
    tasaMensual: [null as number | null],
    puntosPorMora: [null as number | null],
    pagos: this.fb.array([]),
  });

  private readonly submit$ = new Subject<void>();

  get pagosArray(): FormArray {
    return this.form.get('pagos') as FormArray;
  }

  constructor() {
    this.wireSubmit();
  }

  ngOnInit(): void {
    this.cargarListasBase();

    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.contratoId.set(+id);
      this.cargarContrato(+id);
    } else {
      const nav = this.router.getCurrentNavigation();
      const state = (nav?.extras.state ?? history.state) as
        | { editDto?: ContratoPasivoEditDto; lineaCreditoNombre?: string }
        | undefined;
      if (state?.editDto) {
        this.populateForm(state.editDto, state.lineaCreditoNombre);
      }
    }

    this.form.get('tipoTasa')!.valueChanges.pipe(
      tap(() => {
        this.form.patchValue({ idTasa: null, tasa: null, tasaBase: null }, { emitEvent: false });
        this.tasasOrdinarias.set([]);
      }),
      filter((esVariable): esVariable is boolean => esVariable !== null && esVariable !== undefined),
      switchMap((esVariable) =>
        this.selectSvc.getTasas(esVariable).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            if (!wasHandledByInterceptor(err)) {
              this.utilsSvc.showNotification('Error', 'Error al cargar tasas', 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => this.tasasOrdinarias.set(res.data ?? []));

    this.form.get('idTasa')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((idTasa) => {
      if (idTasa == null) {
        this.form.patchValue({ tasa: null, tasaBase: null }, { emitEvent: false });
        return;
      }
      const item = this.tasasOrdinarias().find((t) => t.value === idTasa);
      if (item) {
        const rate = item.valueDecimal ?? null;
        this.form.patchValue({ tasa: rate, tasaBase: rate }, { emitEvent: false });
      }
    });

    this.form.get('tipoTasaMora')!.valueChanges.pipe(
      tap(() => {
        this.form.patchValue({ idTasaMora: null, tasaMora: null, tasaBaseMora: null }, { emitEvent: false });
        this.tasasMora.set([]);
      }),
      filter((esVariable): esVariable is boolean => esVariable !== null && esVariable !== undefined),
      switchMap((esVariable) =>
        this.selectSvc.getTasas(esVariable).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            if (!wasHandledByInterceptor(err)) {
              this.utilsSvc.showNotification('Error', 'Error al cargar tasas', 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => this.tasasMora.set(res.data ?? []));

    this.form.get('idTasaMora')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((idTasaMora) => {
      if (idTasaMora == null) {
        this.form.patchValue({ tasaMora: null, tasaBaseMora: null }, { emitEvent: false });
        return;
      }
      const item = this.tasasMora().find((t) => t.value === idTasaMora);
      if (item) {
        const rate = item.valueDecimal ?? null;
        this.form.patchValue({ tasaMora: rate, tasaBaseMora: rate }, { emitEvent: false });
      }
    });

    this.form.get('idTipoCredito')!.valueChanges.pipe(
      filter((id): id is number => !!id),
      tap(() => this.loadingClave.set(true)),
      switchMap((idTipoCredito) =>
        this.contratosSvc.getClaveContrato(idTipoCredito).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.loadingClave.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.utilsSvc.showNotification('Error', 'Error al obtener la clave del contrato', 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.loadingClave.set(false);
      if (res.success && res.data) {
        this.form.patchValue({ contrato: res.data }, { emitEvent: false });
      }
    });

    this.form.get('noPagosIrregulares')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((n) => this.actualizarPagosIrregulares(n ?? 0));

    this.form.get('idTipoTablaAmortiza')!.valueChanges.pipe(
      tap(() => {
        this.tipoCapitalizacionList.set([]);
        this.tipoPagoCapitalList.set([]);
        this.periodicidadesCapitalizacion.set([]);
        this.esCapitalizable.set(false);
        this.form.patchValue(
          { idTipoCapitalizacion: null, idTipoPagoCapital: null, idPeriodicidad_TTA: null, noPagosIrregulares: null },
          { emitEvent: false },
        );
        this.form.get('idTipoCapitalizacion')!.disable({ emitEvent: false });
        this.form.get('idTipoPagoCapital')!.disable({ emitEvent: false });
        this.form.get('idPeriodicidad_TTA')!.disable({ emitEvent: false });
        this.form.get('noPagosIrregulares')!.disable({ emitEvent: false });
        this.actualizarPagosIrregulares(0);
      }),
      filter((id): id is number => !!id),
      tap(() => this.loadingTablaInfo.set(true)),
      switchMap((id) => {
        const periodicidad$ =
          id === 1
            ? this.selectSvc.getPeriodicidadSelectList()
            : this.selectSvc.getPeriodicidadTTASelectList(id);
        return forkJoin({
          tablaInfo: this.contratosSvc.getTipoTablaAmortizaInfo(id).pipe(timeout(30_000)),
          periodicidad: periodicidad$.pipe(timeout(30_000)),
        }).pipe(
          catchError((err: unknown) => {
            this.loadingTablaInfo.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.utilsSvc.showNotification('Error', 'Error al cargar información de tabla', 'error');
            }
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(({ tablaInfo: res, periodicidad: perRes }) => {
      this.loadingTablaInfo.set(false);
      this.periodicidadesCapitalizacion.set(perRes.data ?? []);
      if (res.success && res.data) {
        const data = res.data;
        this.esCapitalizable.set(data.esCapitalizable ?? false);
        this.tipoCapitalizacionList.set(data.tipoCapitalizacion ?? []);
        this.tipoPagoCapitalList.set(data.tipoPagoCapital ?? []);
        if (data.esCapitalizable) {
          this.form.get('idTipoCapitalizacion')!.enable({ emitEvent: false });
        }
        this.form.get('idTipoPagoCapital')!.enable({ emitEvent: false });
      }
    });

    this.form.get('idTipoPagoCapital')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((id) => {
      this.form.patchValue({ idPeriodicidad_TTA: null, noPagosIrregulares: null }, { emitEvent: false });
      this.form.get('idPeriodicidad_TTA')!.disable({ emitEvent: false });
      this.form.get('noPagosIrregulares')!.disable({ emitEvent: false });
      this.actualizarPagosIrregulares(0);
      if (!id) return;
      const item = this.tipoPagoCapitalList().find((t) => t.value === id);
      if (item?.text?.toLowerCase().includes('irregular')) {
        this.form.get('noPagosIrregulares')!.enable({ emitEvent: false });
      } else {
        this.form.get('idPeriodicidad_TTA')!.enable({ emitEvent: false });
      }
    });
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  get puedeGuardar(): boolean {
    if (!this.isEditMode()) return true;
    return this.form.get('idEstatusContrato')?.value === 1;
  }

  onSubmit(): void {
    if (!this.puedeGuardar) {
      this.utilsSvc.showNotification('Aviso', 'Solo se pueden modificar contratos con estatus Activo (1).', 'warning');
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      const errors: string[] = [];
      Object.entries(this.fieldLabels).forEach(([field, label]) => {
        const ctrl = this.form.get(field);
        if (ctrl?.invalid) errors.push(`${label} es requerido.`);
      });
      this.pagosArray.controls.forEach((pg, i) => {
        if (pg.get('fecVencimiento')?.invalid) {
          errors.push(`Pago ${i + 1}: Fecha de Vencimiento es requerida.`);
        }
      });
      this.formErrors.set(errors);
      return;
    }
    this.formErrors.set([]);
    this.isSaving.set(true);
    this.submit$.next();
  }

  onCancelar(): void {
    this.router.navigate(['/operaciones/contratos-pasivos']);
  }

  onActivar(): void {
    const contrato = this.form.get('contrato')?.value ?? '';
    this.router.navigate(
      ['/operaciones/contratos-pasivos/activar', this.contratoId()],
      { state: { contrato } },
    );
  }

  cargarTablaAmortizacion(idContrato: number): void {
    this.loadingTablaAmortizacion.set(true);
    this.contratosSvc.getTablaAmortizacionByTipo(idContrato).pipe(
      timeout(30_000),
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        this.loadingTablaAmortizacion.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar la tabla de amortización', 'error');
        }
        return EMPTY;
      }),
    ).subscribe((res) => {
      this.loadingTablaAmortizacion.set(false);
      this.tablaAmortizacion.set(res.data ?? []);
    });
  }

  private wireSubmit(): void {
    this.submit$.pipe(
      exhaustMap(() => {
        const dto = this.buildDto();
        const op$ = this.isEditMode()
          ? this.contratosSvc.updateContrato(this.contratoId()!, dto)
          : this.contratosSvc.createContrato(dto);
        return op$.pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.isSaving.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.formErrors.set(this.errorSvc.parseError(err));
            }
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.isSaving.set(false);
      if (res.success === false) {
        const msg = res.errors?.[0] ?? res.message ?? 'Error al guardar el contrato';
        this.formErrors.set([msg]);
      } else {
        this.formErrors.set([]);
        if (this.isEditMode()) {
          this.utilsSvc.showNotification('Éxito', 'Contrato actualizado correctamente', 'success');
          this.cargarTablaAmortizacion(this.contratoId()!);
        } else {
          const nuevoId = res.data as number;
          this.utilsSvc.showNotification('Éxito', 'Contrato creado correctamente', 'success');
          this.router.navigate(['/operaciones/contratos-pasivos/edit', nuevoId]);
        }
      }
    });
  }

  private cargarListasBase(): void {
    forkJoin({
      tiposCredito: this.selectSvc.getTipoCreditoSelectList().pipe(timeout(30_000)),
      monedas: this.selectSvc.getMonedas().pipe(timeout(30_000)),
      periodicidades: this.selectSvc.getPeriodicidadSelectList().pipe(timeout(30_000)),
      tasasIva: this.selectSvc.getTasaIvaSelectList().pipe(timeout(30_000)),
      estatusContrato: this.selectSvc.getEstatusContratoSelectList().pipe(timeout(30_000)),
      tiposTabla: this.configuracionesSvc.apiConfiguracionesTipoTablaAmortizaGet(undefined, 1, 100).pipe(timeout(30_000)),
    }).pipe(
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar listas de selección', 'error');
        }
        return EMPTY;
      }),
    ).subscribe(({ tiposCredito, monedas, periodicidades, tasasIva, estatusContrato, tiposTabla }) => {
      this.tiposCredito.set(tiposCredito.data ?? []);
      this.monedas.set(monedas.data ?? []);
      this.periodicidades.set(periodicidades.data ?? []);
      this.tasasIva.set(tasasIva.data ?? []);
      this.estatusContratoList.set(estatusContrato.data ?? []);
      this.tiposTabla.set(tiposTabla.data?.results ?? []);
    });
  }

  private cargarTasas(esVariable: boolean, tipo: 'ordinaria' | 'mora'): void {
    this.selectSvc.getTasas(esVariable).pipe(
      timeout(30_000),
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar tasas', 'error');
        }
        return EMPTY;
      }),
    ).subscribe((res) => {
      if (tipo === 'ordinaria') this.tasasOrdinarias.set(res.data ?? []);
      else this.tasasMora.set(res.data ?? []);
    });
  }

  private cargarContrato(id: number): void {
    this.isLoading.set(true);
    this.contratosSvc.getContratoById(id).pipe(
      timeout(30_000),
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error de conexión al cargar el contrato', 'error');
        }
        this.router.navigate(['/operaciones/contratos-pasivos']);
        return EMPTY;
      }),
    ).subscribe((res) => {
      this.isLoading.set(false);
      if (res.success && res.data) {
        const d = res.data;

        if (d.tipoTasa !== null && d.tipoTasa !== undefined) {
          this.cargarTasas(d.tipoTasa === true, 'ordinaria');
        }
        if (d.tipoTasaMora !== null && d.tipoTasaMora !== undefined) {
          this.cargarTasas(d.tipoTasaMora === true, 'mora');
        }

        this.form.patchValue(
          {
            fondeador: d.fondeador ?? '',
            lineaCreditoNombre: d.lineaCredito ?? '',
            maxCapitalDisponible: d.maxCapitalDisponible ?? null,
            idTipoCredito: d.idTipoCredito ?? null,
            contrato: d.contrato ?? '',
            idEstatusContrato: d.idEstatusContrato ?? null,
            estatusContrato:
              this.estatusContratoList().find((e) => e.value === d.idEstatusContrato)?.text ?? '',
            capitalFinanciado: d.capitalFinanciado ?? null,
            plazo: d.plazo ?? null,
            idMoneda: d.idMoneda ?? null,
            idPeriodicidad: d.idPeriodicidad ?? null,
            tipoTasa: d.tipoTasa ?? null,
            idTasa: d.idTasa ?? null,
            tasa: d.tasa ?? null,
            tasaBase: d.tasaBase ?? null,
            tasaIva: d.tasaIva ?? null,
            puntosMas: d.puntosMas ?? null,
            puntosPor: d.puntosPor ?? null,
            tipoTasaMora: d.tipoTasaMora ?? null,
            idTasaMora: d.idTasaMora ?? null,
            tasaMora: d.tasaMora ?? null,
            tasaBaseMora: d.tasaBaseMora ?? null,
            puntosMasMora: d.puntosMasMora ?? null,
            factorMora: d.factorMora ?? null,
            fecInicioContrato: this.toDateInput(d.fecInicioContrato),
            fechaFirmaContrato: this.toDateInput(d.fechaFirmaContrato),
            fecPrimeraRenta: this.toDateInput(d.fecPrimeraRenta),
            fecActivacion: this.toDateInput(d.fecActivacion),
            fecFinContrato: this.toDateInput(d.fecFinContrato),
            idLineaCredito: d.idLineaCredito ?? null,
            idFondeador: d.idFondeador ?? null,
            idTipoTablaAmortiza: d.idTipoTablaAmortiza ?? null,
            idTipoPagoCapital: d.idTipoPagoCapital ?? null,
            idTipoCapitalizacion: d.idTipoCapitalizacion ?? null,
            noPagosIrregulares: d.noPagosIrregulares ?? null,
          },
          { emitEvent: false },
        );

        if (d.idTipoTablaAmortiza) {
          this.cargarInfoTablaAmortiza(
            d.idTipoTablaAmortiza,
            d.idTipoCapitalizacion,
            d.idTipoPagoCapital,
            d.idPeriodicidad_TTA,
          );
        }

        this.cargarTablaAmortizacion(id);

        if (d.pagos?.length) {
          d.pagos.forEach((p) =>
            this.pagosArray.push(
              this.fb.group({
                noPago: [p.noPago ?? null],
                capital: [p.capital ?? null],
                fecVencimiento: [this.toDateInput(p.fecVencimiento), Validators.required],
              }),
            ),
          );
        }
      } else {
        this.utilsSvc.showNotification('Error', 'No se pudo cargar el contrato', 'error');
        this.router.navigate(['/operaciones/contratos-pasivos']);
      }
    });
  }

  private populateForm(d: ContratoPasivoEditDto, lineaCreditoNombre?: string): void {
    if (d.tipoTasa !== null && d.tipoTasa !== undefined) {
      this.cargarTasas(d.tipoTasa === true, 'ordinaria');
    }

    this.form.patchValue(
      {
        fondeador: d.fondeador ?? '',
        lineaCreditoNombre: d.lineaCredito ?? '',
        maxCapitalDisponible: d.maxCapitalDisponible ?? null,
        idTipoCredito: d.idTipoCredito ?? null,
        contrato: d.contrato ?? '',
        capitalFinanciado: d.capitalFinanciado ?? null,
        plazo: d.plazo ?? null,
        idMoneda: d.idMoneda ?? null,
        idPeriodicidad: d.idPeriodicidad ?? null,
        tipoTasa: d.tipoTasa ?? null,
        idTasa: d.idTasa ?? null,
        tasa: d.tasa ?? null,
        tasaBase: d.tasaBase ?? null,
        tasaIva: d.tasaIva ?? null,
        puntosMas: d.puntosMas ?? null,
        puntosPor: d.puntosPor ?? null,
        idTasaMora: d.idTasaMora ?? null,
        tasaMora: d.tasaMora ?? null,
        tasaBaseMora: d.tasaBaseMora ?? null,
        puntosMasMora: d.puntosMasMora ?? null,
        factorMora: d.factorMora ?? null,
        fecInicioContrato: this.toDateInput(d.fecInicioContrato),
        fechaFirmaContrato: this.toDateInput(d.fechaFirmaContrato),
        fecPrimeraRenta: this.toDateInput(d.fecPrimeraRenta),
        fecActivacion: this.toDateInput(d.fecActivacion),
        fecFinContrato: this.toDateInput(d.fecFinContrato),
        idLineaCredito: d.idLineaCredito ?? null,
        idFondeador: d.idFondeador ?? null,
        tasaEsVariable: d.tasaEsVariable ?? null,
        idTipoTablaAmortiza: d.idTipoTablaAmortiza ?? null,
        versionTabla: d.versionTabla ?? null,
        idTipoCalculoTasaVariable: d.idTipoCalculoTasaVariable ?? null,
        idTipoMantenimiento: d.idTipoMantenimiento ?? null,
        idTipoPagoCapital: d.idTipoPagoCapital ?? null,
        idTipoCapitalizacion: d.idTipoCapitalizacion ?? null,
        nroRentasDepositoGarantia: d.nroRentasDepositoGarantia ?? null,
        factorFIRA: d.factorFIRA ?? null,
        tasaMensual: d.tasaMensual ?? null,
        noPagosIrregulares: d.noPagosIrregulares ?? null,
      },
      { emitEvent: false },
    );

    if (d.idTipoTablaAmortiza) {
      this.cargarInfoTablaAmortiza(
        d.idTipoTablaAmortiza,
        d.idTipoCapitalizacion,
        d.idTipoPagoCapital,
        d.idPeriodicidad_TTA,
      );
    }

    if (d.pagos?.length) {
      d.pagos.forEach((p) =>
        this.pagosArray.push(
          this.fb.group({
            noPago: [p.noPago ?? null],
            capital: [p.capital ?? null],
            fecVencimiento: [this.toDateInput(p.fecVencimiento), Validators.required],
          }),
        ),
      );
    }
  }

  private actualizarPagosIrregulares(n: number): void {
    while (this.pagosArray.length > n) this.pagosArray.removeAt(this.pagosArray.length - 1);
    while (this.pagosArray.length < n) {
      this.pagosArray.push(
        this.fb.group({
          noPago: [this.pagosArray.length + 1],
          capital: [null as number | null],
          fecVencimiento: ['', Validators.required],
        }),
      );
    }
  }

  private cargarInfoTablaAmortiza(
    id: number,
    idTipoCapitalizacion?: number | null,
    idTipoPagoCapital?: number | null,
    idPeriodicidad_TTA?: number | null,
  ): void {
    this.loadingTablaInfo.set(true);
    const periodicidad$ =
      id === 1
        ? this.selectSvc.getPeriodicidadSelectList()
        : this.selectSvc.getPeriodicidadTTASelectList(id);

    forkJoin({
      tablaInfo: this.contratosSvc.getTipoTablaAmortizaInfo(id).pipe(timeout(30_000)),
      periodicidad: periodicidad$.pipe(timeout(30_000)),
    }).pipe(
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        this.loadingTablaInfo.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar información de tabla', 'error');
        }
        return EMPTY;
      }),
    ).subscribe(({ tablaInfo: res, periodicidad: perRes }) => {
      this.loadingTablaInfo.set(false);
      this.periodicidadesCapitalizacion.set(perRes.data ?? []);
      if (res.success && res.data) {
        const data = res.data;
        this.esCapitalizable.set(data.esCapitalizable ?? false);
        this.tipoCapitalizacionList.set(data.tipoCapitalizacion ?? []);
        this.tipoPagoCapitalList.set(data.tipoPagoCapital ?? []);

        if (data.esCapitalizable) {
          this.form.get('idTipoCapitalizacion')!.enable({ emitEvent: false });
        }
        this.form.get('idTipoPagoCapital')!.enable({ emitEvent: false });

        const patch: {
          idTipoCapitalizacion?: number | null;
          idTipoPagoCapital?: number | null;
          idPeriodicidad_TTA?: number | null;
        } = {};

        if (idTipoCapitalizacion != null) patch.idTipoCapitalizacion = idTipoCapitalizacion;
        if (idTipoPagoCapital != null) {
          patch.idTipoPagoCapital = idTipoPagoCapital;
          const item = (data.tipoPagoCapital ?? []).find((t) => t.value === idTipoPagoCapital);
          if (item?.text?.toLowerCase().includes('irregular')) {
            this.form.get('noPagosIrregulares')!.enable({ emitEvent: false });
          } else {
            this.form.get('idPeriodicidad_TTA')!.enable({ emitEvent: false });
            if (idPeriodicidad_TTA != null) patch.idPeriodicidad_TTA = idPeriodicidad_TTA;
          }
        }
        if (Object.keys(patch).length > 0) {
          this.form.patchValue(patch, { emitEvent: false });
        }
      }
    });
  }

  private buildDto(): ContratoPasivoEditDto {
    const v = this.form.getRawValue();
    return {
      fondeador: v.fondeador ?? undefined,
      idLineaCredito: v.idLineaCredito ?? undefined,
      maxCapitalDisponible: v.maxCapitalDisponible ?? undefined,
      idTipoCredito: v.idTipoCredito ?? undefined,
      contrato: v.contrato ?? undefined,
      capitalFinanciado: v.capitalFinanciado ?? undefined,
      plazo: v.plazo ?? undefined,
      idMoneda: v.idMoneda ?? undefined,
      idPeriodicidad: v.idPeriodicidad ?? undefined,
      tipoTasa: v.tipoTasa ?? undefined,
      tasaEsVariable: v.tipoTasa === true,
      idTasa: v.idTasa ?? undefined,
      tasaIva: v.tasaIva ?? undefined,
      tasa: v.tasa ?? undefined,
      tasaBase: v.tasaBase ?? undefined,
      puntosMas: v.puntosMas ?? undefined,
      puntosPor: v.puntosPor ?? undefined,
      idTasaMora: v.idTasaMora ?? undefined,
      tasaMora: v.tasaMora ?? undefined,
      tasaBaseMora: v.tasaBaseMora ?? undefined,
      puntosMasMora: v.puntosMasMora ?? undefined,
      puntosPorMora: v.puntosPorMora ?? undefined,
      factorMora: v.factorMora ?? undefined,
      fecInicioContrato: v.fecInicioContrato ?? '',
      fechaFirmaContrato: v.fechaFirmaContrato ?? undefined,
      fecPrimeraRenta: v.fecPrimeraRenta ?? undefined,
      fecActivacion: v.fecActivacion ?? undefined,
      fecFinContrato: v.fecFinContrato ?? undefined,
      idFondeador: v.idFondeador ?? undefined,
      idTipoTablaAmortiza: v.idTipoTablaAmortiza ?? undefined,
      idTipoCapitalizacion: v.idTipoCapitalizacion ?? undefined,
      idTipoPagoCapital: v.idTipoPagoCapital ?? undefined,
      idPeriodicidad_TTA: v.idPeriodicidad_TTA ?? undefined,
      versionTabla: v.versionTabla ?? undefined,
      idTipoCalculoTasaVariable: v.idTipoCalculoTasaVariable ?? undefined,
      idTipoMantenimiento: v.idTipoMantenimiento ?? undefined,
      nroRentasDepositoGarantia: v.nroRentasDepositoGarantia ?? undefined,
      factorFIRA: v.factorFIRA ?? undefined,
      tasaMensual: v.tasaMensual ?? undefined,
      noPagosIrregulares: v.noPagosIrregulares ?? undefined,
      pagos:
        this.pagosArray.length > 0
        ? (this.pagosArray.getRawValue() as PagoFormValue[]).map((p) => ({
            noPago: p.noPago ?? undefined,
            capital: p.capital ?? undefined,
            fecVencimiento: p.fecVencimiento ?? '',
          }))
        : undefined,
    };
  }

  private toDateInput(val?: string | null): string | null {
    if (!val) return null;
    return val.substring(0, 10);
  }
}
