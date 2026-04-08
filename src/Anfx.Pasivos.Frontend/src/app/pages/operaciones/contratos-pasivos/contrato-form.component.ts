import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
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
  readonly errorSvc = inject(ErrorHandlerService);
  // ── Estado ────────────────────────────────────────────────────
  isEditMode = signal(false);
  contratoId = signal<number | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);
  loadingClave = signal(false);

  // ── Listas ───────────────────────────────────────────────────
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

  private readonly fieldLabels: Record<string, string> = {
    idTipoCredito: 'Tipo de Crédito',
    contrato: 'Clave de Contrato',
    capitalFinanciado: 'Capital Financiado',
    fecInicioContrato: 'Fecha de Alta de Contrato',
  };

  // Opciones estáticas para Tipo de Tasa
  readonly tipoTasaOpciones = [
    { value: false, text: 'Fija' },
    { value: true, text: 'Variable' },
  ];

  estatusContratoList = signal<SelectItemDto[]>([]);

  // ── Formulario ───────────────────────────────────────────────
  form = this.fb.group({
    // Sección 1 – Información de Línea de Crédito (readonly)
    fondeador: [{ value: '', disabled: true }],
    lineaCreditoNombre: [{ value: '', disabled: true }],
    maxCapitalDisponible: [{ value: null as number | null, disabled: true }],

    // Sección 2 – Datos del Contrato
    idTipoCredito: [null as number | null, Validators.required],
    contrato: ['', Validators.required],
    estatusContrato: [{ value: '', disabled: true }],
    idEstatusContrato: [{ value: null as number | null, disabled: true }],

    // Sección 3 – Datos Base
    capitalFinanciado: [null as number | null, [Validators.required, Validators.min(0.01)]],
    plazo: [null as number | null],
    idMoneda: [{ value: null as number | null, disabled: true }],
    idPeriodicidad: [{ value: null as number | null, disabled: true }],

    // Sección 4 – Tasa Ordinaria
    tipoTasa: [null as boolean | null],
    idTasa: [null as number | null],
    tasaIva: [null as number | null], // valueDecimal del ítem seleccionado
    tasa: [null as number | null], // Valor Tasa (editable)
    tasaBase: [{ value: null as number | null, disabled: true }], // readonly
    puntosMas: [null as number | null], // Puntos Adicionales
    puntosPor: [null as number | null], // Factor

    // Sección 5 – Tasa Mora
    tipoTasaMora: [null as boolean | null], // control UI, no va en DTO
    idTasaMora: [null as number | null],
    tasaMora: [null as number | null], // Valor Tasa (editable)
    tasaBaseMora: [{ value: null as number | null, disabled: true }], // readonly
    puntosMasMora: [null as number | null], // Puntos Adicionales Mora
    factorMora: [null as number | null], // Factor Adicional Mora

    // Sección 6 – Fechas
    fecInicioContrato: ['', Validators.required], // Alta Contrato
    fechaFirmaContrato: [null as string | null],
    fecPrimeraRenta: [null as string | null], // Primer Vencimiento
    fecActivacion: [{ value: null as string | null, disabled: true }],
    fecFinContrato: [{ value: null as string | null, disabled: true }],

    // Sección 7 – Configuración de Tabla Amortización
    idTipoTablaAmortiza: [null as number | null],
    idTipoCapitalizacion: [{ value: null as number | null, disabled: true }],
    idTipoPagoCapital: [{ value: null as number | null, disabled: true }],
    idPeriodicidad_TTA: [{ value: null as number | null, disabled: true }],
    noPagosIrregulares: [{ value: null as number | null, disabled: true }],

    // Campos del DTO no visibles en formulario (enviados al backend)
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

  get pagosArray(): FormArray {
    return this.form.get('pagos') as FormArray;
  }

  // ── Lifecycle ────────────────────────────────────────────────

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

    // Tipo de Tasa Ordinaria cambia → cargar lista de tasas correspondiente
    this.form.get('tipoTasa')!.valueChanges.subscribe((esVariable) => {
      this.form.patchValue({ idTasa: null, tasa: null, tasaBase: null }, { emitEvent: false });
      this.tasasOrdinarias.set([]);
      if (esVariable !== null && esVariable !== undefined) {
        this.cargarTasas(!!esVariable, 'ordinaria');
      }
    });

    // Tasa Ordinaria seleccionada → completar Valor Tasa y Valor Tasa Base
    this.form.get('idTasa')!.valueChanges.subscribe((idTasa) => {
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

    // Tipo de Tasa Mora cambia → cargar lista de tasas mora
    this.form.get('tipoTasaMora')!.valueChanges.subscribe((esVariable) => {
      this.form.patchValue(
        { idTasaMora: null, tasaMora: null, tasaBaseMora: null },
        { emitEvent: false },
      );
      this.tasasMora.set([]);
      if (esVariable !== null && esVariable !== undefined) {
        this.cargarTasas(!!esVariable, 'mora');
      }
    });

    // Tasa Mora seleccionada → completar Valor Tasa Mora y Valor Tasa Base Mora
    this.form.get('idTasaMora')!.valueChanges.subscribe((idTasaMora) => {
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

    // Tipo de Crédito cambia → obtener clave del contrato
    this.form.get('idTipoCredito')!.valueChanges.subscribe((idTipoCredito) => {
      if (!idTipoCredito) return;
      this.loadingClave.set(true);
      this.contratosSvc.getClaveContrato(idTipoCredito).subscribe({
        next: (res) => {
          this.loadingClave.set(false);
          if (res.success && res.data) {
            this.form.patchValue({ contrato: res.data }, { emitEvent: false });
          }
        },
        error: (err) => {
          this.loadingClave.set(false);
          if (!wasHandledByInterceptor(err)) {
            this.utilsSvc.showNotification(
              'Error',
              'Error al obtener la clave del contrato',
              'error',
            );
          }
        },
      });
    });

    // Pagos irregulares
    this.form.get('noPagosIrregulares')!.valueChanges.subscribe((n) => {
      this.actualizarPagosIrregulares(n ?? 0);
    });

    // Tipo de Tabla Amortización cambia → cargar listas de Capitalización y Tipo Pago
    this.form.get('idTipoTablaAmortiza')!.valueChanges.subscribe((id) => {
      this.tipoCapitalizacionList.set([]);
      this.tipoPagoCapitalList.set([]);
      this.periodicidadesCapitalizacion.set([]);
      this.esCapitalizable.set(false);
      this.form.patchValue(
        {
          idTipoCapitalizacion: null,
          idTipoPagoCapital: null,
          idPeriodicidad_TTA: null,
          noPagosIrregulares: null,
        },
        { emitEvent: false },
      );
      this.form.get('idTipoCapitalizacion')!.disable({ emitEvent: false });
      this.form.get('idTipoPagoCapital')!.disable({ emitEvent: false });
      this.form.get('idPeriodicidad_TTA')!.disable({ emitEvent: false });
      this.form.get('noPagosIrregulares')!.disable({ emitEvent: false });
      this.actualizarPagosIrregulares(0);

      if (!id) return;
      this.cargarInfoTablaAmortiza(id);
    });

    // Tipo de Pago Capital cambia → habilitar Periodicidad Cap o Nro. Pagos
    this.form.get('idTipoPagoCapital')!.valueChanges.subscribe((id) => {
      this.form.patchValue(
        { idPeriodicidad_TTA: null, noPagosIrregulares: null },
        { emitEvent: false },
      );
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

  // ── Helpers template ─────────────────────────────────────────

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  get puedeGuardar(): boolean {
    if (!this.isEditMode()) return true;
    return this.form.get('idEstatusContrato')?.value === 1;
  }

  // ── Acciones ─────────────────────────────────────────────────

  onSubmit(): void {
    if (!this.puedeGuardar) {
      this.utilsSvc.showNotification(
        'Aviso',
        'Solo se pueden modificar contratos con estatus Activo (1).',
        'warning',
      );
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      const errors: string[] = [];
      Object.entries(this.fieldLabels).forEach(([field, label]) => {
        const ctrl = this.form.get(field);
        if (ctrl?.invalid) errors.push(`${label} es requerido.`);
      });
      // Pagos irregulares con fecha vacía
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
    const dto = this.buildDto();
    console.log(dto);
    const op$ = this.isEditMode()
      ? this.contratosSvc.updateContrato(this.contratoId()!, dto)
      : this.contratosSvc.createContrato(dto);

    op$.subscribe({
      next: (res: any) => {
        console.log(res);
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
      },
      error: (err: unknown) => {
        this.isSaving.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.formErrors.set(this.errorSvc.parseError(err));
          // const msg = err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al guardar el contrato';
          // this.utilsSvc.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  onCancelar(): void {
    this.router.navigate(['/operaciones/contratos-pasivos']);
  }

  // ── Private ──────────────────────────────────────────────────

  private cargarListasBase(): void {
    forkJoin({
      tiposCredito: this.selectSvc.getTipoCreditoSelectList(),
      monedas: this.selectSvc.getMonedas(),
      periodicidades: this.selectSvc.getPeriodicidadSelectList(),
      tasasIva: this.selectSvc.getTasaIvaSelectList(),
      estatusContrato: this.selectSvc.getEstatusContratoSelectList(),
      tiposTabla: this.configuracionesSvc.apiConfiguracionesTipoTablaAmortizaGet(undefined, 1, 100),
    }).subscribe({
      next: ({ tiposCredito, monedas, periodicidades, tasasIva, estatusContrato, tiposTabla }) => {
        this.tiposCredito.set(tiposCredito.data ?? []);
        this.monedas.set(monedas.data ?? []);
        this.periodicidades.set(periodicidades.data ?? []);
        this.tasasIva.set(tasasIva.data ?? []);
        this.estatusContratoList.set(estatusContrato.data ?? []);
        this.tiposTabla.set(tiposTabla.data?.results ?? []);
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar listas de selección', 'error');
        }
      },
    });
  }

  private cargarTasas(esVariable: boolean, tipo: 'ordinaria' | 'mora'): void {
    this.selectSvc.getTasas(esVariable).subscribe({
      next: (res) => {
        if (tipo === 'ordinaria') this.tasasOrdinarias.set(res.data ?? []);
        else this.tasasMora.set(res.data ?? []);
      },
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar tasas', 'error');
        }
      },
    });
  }

  private cargarContrato(id: number): void {
    this.isLoading.set(true);
    this.contratosSvc.getContratoById(id).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          const d = res.data;

          // Cargar tasas antes de patch para que los selects estén disponibles
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
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification(
            'Error',
            'Error de conexión al cargar el contrato',
            'error',
          );
        }
        this.router.navigate(['/operaciones/contratos-pasivos']);
      },
    });
  }

  private populateForm(d: ContratoPasivoEditDto, lineaCreditoNombre?: string): void {
    // Pre-cargar tasas si ya viene definido el tipo
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
      tablaInfo: this.contratosSvc.getTipoTablaAmortizaInfo(id),
      periodicidad: periodicidad$,
    }).subscribe({
      next: ({ tablaInfo: res, periodicidad: perRes }) => {
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

          // Restore saved values
          const patch: Partial<Record<string, unknown>> = {};
          if (idTipoCapitalizacion != null) patch['idTipoCapitalizacion'] = idTipoCapitalizacion;
          if (idTipoPagoCapital != null) {
            patch['idTipoPagoCapital'] = idTipoPagoCapital;
            const item = (data.tipoPagoCapital ?? []).find((t) => t.value === idTipoPagoCapital);
            if (item?.text?.toLowerCase().includes('irregular')) {
              this.form.get('noPagosIrregulares')!.enable({ emitEvent: false });
            } else {
              this.form.get('idPeriodicidad_TTA')!.enable({ emitEvent: false });
              if (idPeriodicidad_TTA != null) patch['idPeriodicidad_TTA'] = idPeriodicidad_TTA;
            }
          }
          if (Object.keys(patch).length > 0) {
            this.form.patchValue(patch as any, { emitEvent: false });
          }
        }
      },
      error: (err) => {
        this.loadingTablaInfo.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar información de tabla', 'error');
        }
      },
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
          ? this.pagosArray.getRawValue().map((p: any) => ({
              noPago: p.noPago,
              capital: p.capital,
              fecVencimiento: p.fecVencimiento ?? '',
            }))
          : undefined,
    };
  }

  private toDateInput(val?: string | null): string | null {
    if (!val) return null;
    return val.substring(0, 10);
  }

  cargarTablaAmortizacion(idContrato: number): void {
    this.loadingTablaAmortizacion.set(true);
    this.contratosSvc.getTablaAmortizacionByTipo(idContrato).subscribe({
      next: (res) => {
        this.loadingTablaAmortizacion.set(false);
        this.tablaAmortizacion.set(res.data ?? []);
      },
      error: (err) => {
        this.loadingTablaAmortizacion.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar la tabla de amortización', 'error');
        }
      },
    });
  }
}
FormErrorsComponent;
