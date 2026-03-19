import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, switchMap, of } from 'rxjs';
import { OperacionesService } from '../../../../api/services/operaciones.service';
import { SelectListsService } from '../../../../api/services/selectLists.service';
import { AnticipoDto } from '../../../../api/models/anticipoDto';
import { AnticipoConfigDto } from '../../../../api/models/anticipoConfigDto';
import { SelectItemDto } from '../../../../api/models/selectItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-anticipo',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './anticipo.component.html',
})
export class AnticipoComponent implements OnInit {
  private readonly operacionesSvc = inject(OperacionesService);
  private readonly selectSvc      = inject(SelectListsService);
  private readonly utilsService   = inject(UtilsService);
  private readonly fb             = inject(FormBuilder);

  // ── Catálogos ────────────────────────────────────────────────
  tiposTerminacion = signal<SelectItemDto[]>([]);
  tiposReduccion   = signal<SelectItemDto[]>([]);
  loadingCatalogos = signal(false);

  // ── Estado búsqueda ──────────────────────────────────────────
  contratoBusqueda = '';
  buscando         = signal(false);
  errorBusqueda    = signal<string | null>(null);

  // ── Datos cargados ───────────────────────────────────────────
  datosAnticipo    = signal<AnticipoDto | null>(null);
  config           = signal<AnticipoConfigDto | null>(null);
  guardando        = signal(false);
  calculandoConfig = signal(false);

  // ── Formulario ───────────────────────────────────────────────
  form = this.fb.group({
    fechaAnticipo:     ['',   Validators.required],
    montoAnticipo:     [null as number | null, [Validators.required, Validators.min(0.01)]],
    idTipoTerminacion: [null as number | null, Validators.required],
    idTipoReduccion:   [{ value: null as number | null, disabled: true }, Validators.required],
    montoInteres:      [null as number | null, Validators.required],
    montoIVA_Interes:  [null as number | null, Validators.required],
    montoTotal:        [{ value: null as number | null, disabled: true }],
  });

  constructor() {
    this.form.get('idTipoTerminacion')!.valueChanges.subscribe(idTipo => {
      const ctrlReduccion = this.form.get('idTipoReduccion')!;

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
      reducciones:   this.selectSvc.getTipoReduccion(),
    }).subscribe({
      next: ({ terminaciones, reducciones }) => {
        this.tiposTerminacion.set(terminaciones.data ?? []);
        this.tiposReduccion.set(reducciones.data ?? []);
        this.loadingCatalogos.set(false);
      },
      error: (err) => {
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

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  // ── Búsqueda ─────────────────────────────────────────────────

  onBuscar(): void {
    const contrato = this.contratoBusqueda.trim();
    if (!contrato) return;

    this.buscando.set(true);
    this.errorBusqueda.set(null);
    this.datosAnticipo.set(null);
    this.form.reset();
    this.form.get('idTipoReduccion')!.disable({ emitEvent: false });

    this.operacionesSvc.getAnticipoByContrato(contrato).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.datosAnticipo.set(res.data);
          this.poblarFormulario(res.data);
        } else {
          this.errorBusqueda.set(res.errors?.[0] ?? res.message ?? 'Contrato no encontrado');
        }
        this.buscando.set(false);
      },
      error: (err) => {
        this.buscando.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.errorBusqueda.set('Error de conexión al buscar el contrato');
        }
      },
    });
  }

  // ── Confirmar ────────────────────────────────────────────────

  onConfirmar(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const v    = this.form.getRawValue();
    const base = this.datosAnticipo()!;

    const dto: AnticipoDto = {
      idContrato:        base.idContrato,
      fechaAnticipo:     v.fechaAnticipo!,
      montoAnticipo:     v.montoAnticipo!,
      idTipoTerminacion: v.idTipoTerminacion!,
      idTipoReduccion:   v.idTipoReduccion ?? 0,
      montoInteres:      v.montoInteres!,
      montoIVA_Interes:  v.montoIVA_Interes!,
      montoTotal:        v.montoTotal!,
      esLiquidacion:     base.esLiquidacion ?? false,
      montoPena:         base.montoPena     ?? 0,
      montoIVA_Pena:     base.montoIVA_Pena ?? 0,
    };

    this.guardando.set(true);
    this.operacionesSvc.confirmarAnticipo(dto).subscribe({
      next: (res) => {
        this.guardando.set(false);
        const msg = res?.message ?? 'Anticipo procesado correctamente';
        this.utilsService.showNotification('Éxito', msg, 'success');
        this.datosAnticipo.set(null);
        this.contratoBusqueda = '';
        this.form.reset();
        this.form.get('idTipoReduccion')!.disable({ emitEvent: false });
      },
      error: (err) => {
        this.guardando.set(false);
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al procesar el anticipo';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  // ── Private ──────────────────────────────────────────────────

  private poblarFormulario(d: AnticipoDto): void {
    // Habilitar reducción si el tipo es 1
    if ((d.idTipoTerminacion ?? 0) === 1) {
      this.form.get('idTipoReduccion')!.enable({ emitEvent: false });
    }

    this.form.patchValue({
      fechaAnticipo:     d.fechaAnticipo?.substring(0, 10) ?? '',
      montoAnticipo:     d.montoAnticipo    ?? null,
      idTipoTerminacion: d.idTipoTerminacion ?? null,
      idTipoReduccion:   d.idTipoReduccion  ?? null,
      montoInteres:      d.montoInteres     ?? 0,
      montoIVA_Interes:  d.montoIVA_Interes ?? 0,
      montoTotal:        d.montoTotal       ?? null,
    }, { emitEvent: false });

    // Disparar config si ya viene con tipo
    if (d.idTipoTerminacion && d.idContrato) {
      this.cargarConfig(d.idTipoTerminacion, d.idContrato);
    }
  }

  private cargarConfig(idTipoTerminacion: number, idContrato: number): void {
    this.calculandoConfig.set(true);

    this.operacionesSvc.getAnticipoConfig(idTipoTerminacion, idContrato).pipe(
      switchMap((res) => {
        if (!res.success || !res.data) return of({ config: null, interes: null });

        const cfg = res.data as AnticipoConfigDto;
        this.config.set(cfg);
        this.datosAnticipo.update(d => d ? { ...d, esLiquidacion: cfg.esLiquidacion } : d);

        // Si es liquidación: precargar montos del config en el formulario
        if (cfg.esLiquidacion) {
          this.form.patchValue({
            montoAnticipo: cfg.montoAnticipo ?? this.form.get('montoAnticipo')!.value,
            montoTotal:    cfg.montoTotal    ?? this.form.get('montoTotal')!.value,
          });
        }

        // Si calcula interés: llamar al endpoint con los datos actuales del formulario
        if (cfg.calculaInteres) {
          const fechaAnticipo = this.form.get('fechaAnticipo')!.value as string;
          const montoAnticipo = cfg.esLiquidacion
            ? (cfg.montoAnticipo ?? this.form.get('montoAnticipo')!.value as number ?? 0)
            : (this.form.get('montoAnticipo')!.value as number ?? 0);

          if (fechaAnticipo && montoAnticipo) {
            return this.operacionesSvc.getInteres(idContrato, fechaAnticipo, montoAnticipo).pipe(
              switchMap(interesRes => of({ config: cfg, interes: interesRes }))
            );
          }
        }

        return of({ config: cfg, interes: null });
      })
    ).subscribe({
      next: ({ config: cfg, interes }) => {
        if (cfg && interes?.success && interes.data != null) {
          const montoInteres    = interes.data as number;
          const montoIVA_Interes = montoInteres * (cfg.porcIVA_Interes ?? 0);
          this.form.patchValue({ montoInteres, montoIVA_Interes });
        }
        this.calculandoConfig.set(false);
      },
      error: (err) => {
        this.calculandoConfig.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar configuración del anticipo', 'error');
        }
      },
    });
  }
}
