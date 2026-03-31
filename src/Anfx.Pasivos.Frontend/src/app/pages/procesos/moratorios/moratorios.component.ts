import { Component, inject, signal, ViewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProcesosService } from 'src/app/core/api/services/procesos.service';
import { ProcesaMoratorioRequest } from 'src/app/core/api/models/procesaMoratorioRequest';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { ConfirmModalComponent } from '@shared/components/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-moratorios',
  standalone: true,
  imports: [ReactiveFormsModule, ConfirmModalComponent],
  templateUrl: './moratorios.component.html',
})
export class MoratoriosComponent {
  private readonly procesosSvc  = inject(ProcesosService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb           = inject(FormBuilder);

  @ViewChild('confirmModal') confirmModal!: ConfirmModalComponent;

  procesando = signal(false);
  resultado  = signal<string | null>(null);
  errores    = signal<string[]>([]);

  form = this.fb.group({
    contratoPasivo:     [null as string | null],
    fechaProcesamiento: ['', Validators.required],
  });

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
    const request: ProcesaMoratorioRequest = {
      fechaProcesamiento: v.fechaProcesamiento!,
      contratoPasivo:     v.contratoPasivo || null,
    };

    this.procesando.set(true);
    this.resultado.set(null);
    this.errores.set([]);
    this.procesosSvc.procesarMoratorios(request).subscribe({
      next: (res) => {
        this.procesando.set(false);
        if (res.success) {
          this.resultado.set(res.data?.procedimiento ?? res.message ?? 'Proceso completado');
          this.errores.set(res.errors ?? []);
          this.utilsService.showNotification('Éxito', res.message ?? 'Proceso completado', 'success');
        } else {
          this.errores.set(res.errors ?? []);
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
    this.resultado.set(null);
    this.errores.set([]);
  }
}
