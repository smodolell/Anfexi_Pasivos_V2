import { Component, OnInit, inject, signal, DestroyRef, ViewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProcesosService } from '../../../../api/services/procesos.service';
import { SelectListsService } from '../../../../api/services/selectLists.service';
import { ProcesaVencimientoRequest } from '../../../../api/models/procesaVencimientoRequest';
import { SelectItemDto } from '../../../../api/models/selectItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '../../../services/utils.service';
import { ConfirmModalComponent } from '../../../shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-vencimiento',
  standalone: true,
  imports: [ReactiveFormsModule, ConfirmModalComponent],
  templateUrl: './vencimiento.component.html',
})
export class VencimientoComponent implements OnInit {
  private readonly procesosSvc  = inject(ProcesosService);
  private readonly selectSvc    = inject(SelectListsService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb           = inject(FormBuilder);
  private readonly destroyRef   = inject(DestroyRef);

  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  // ── Catálogos ────────────────────────────────────────────────
  fondeadores      = signal<SelectItemDto[]>([]);
  contratos        = signal<SelectItemDto[]>([]);
  cargandoContratos = signal(false);

  // ── Estado ───────────────────────────────────────────────────
  procesando       = signal(false);
  totalProcesados  = signal<number | null>(null);

  // ── Formulario ───────────────────────────────────────────────
  form = this.fb.group({
    idFondeador:  [null as number | null],
    idContrato:   [null as number | null],
    fechaInicial: ['', Validators.required],
    fechaFinal:   ['', Validators.required],
  });

  constructor() {
    this.form.get('idFondeador')!.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(idFondeador => {
        this.contratos.set([]);
        this.form.get('idContrato')!.setValue(null, { emitEvent: false });

        if (idFondeador) {
          this.cargandoContratos.set(true);
          this.selectSvc.getContratosPasivosPorFondeador(idFondeador).subscribe({
            next: (res) => {
              this.contratos.set(res.data ?? []);
              this.cargandoContratos.set(false);
            },
            error: (err) => {
              this.cargandoContratos.set(false);
              if (!wasHandledByInterceptor(err)) {
                this.utilsService.showNotification('Error', 'Error al cargar contratos', 'error');
              }
            },
          });
        }
      });
  }

  ngOnInit(): void {
    this.selectSvc.getFondeadoresSelectList().subscribe({
      next: (res) => this.fondeadores.set(res.data ?? []),
      error: (err) => {
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar fondeadores', 'error');
        }
      },
    });
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onProcesar(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.confirmModal.show();
  }

  ejecutarProceso(): void {
    this.confirmModal.hide();
    const v = this.form.getRawValue();
    const request: ProcesaVencimientoRequest = {
      fechaInicial: v.fechaInicial!,
      fechaFinal:   v.fechaFinal!,
      idFondeador:  v.idFondeador  ?? null,
      idContrato:   v.idContrato   ?? null,
    };

    this.procesando.set(true);
    this.totalProcesados.set(null);
    this.procesosSvc.procesarVencimientos(request).subscribe({
      next: (res) => {
        this.procesando.set(false);
        if (res.success) {
          this.totalProcesados.set(res.data?.totalProcesados ?? 0);
          this.utilsService.showNotification('Éxito', res.message ?? 'Proceso completado', 'success');
        } else {
          this.utilsService.showNotification('Error', res.errors?.[0] ?? res.message ?? 'Error al procesar', 'error');
        }
      },
      error: (err) => {
        this.procesando.set(false);
        if (!wasHandledByInterceptor(err)) {
          const msg = err?.error?.message ?? err?.error?.errors?.[0] ?? 'Error al ejecutar el proceso';
          this.utilsService.showNotification('Error', msg, 'error');
        }
      },
    });
  }

  onLimpiar(): void {
    this.form.reset();
    this.contratos.set([]);
    this.totalProcesados.set(null);
  }
}
