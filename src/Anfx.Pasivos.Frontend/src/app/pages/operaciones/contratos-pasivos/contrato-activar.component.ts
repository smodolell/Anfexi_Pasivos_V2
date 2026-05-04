import { Component, OnInit, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EMPTY, Subject, exhaustMap, timeout, catchError } from 'rxjs';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { FormErrorsComponent } from '@shared/components/form-errors/form-error.component';

@Component({
  selector: 'app-contrato-activar',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, FormErrorsComponent],
  templateUrl: './contrato-activar.component.html',
})
export class ContratoActivarComponent implements OnInit {
  private readonly contratosSvc = inject(ContratosService);
  private readonly utilsSvc = inject(UtilsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  contratoId = signal<number>(0);
  contratoNombre = signal<string>('');
  isSaving = signal(false);
  formErrors = signal<string[]>([]);

  form = this.fb.group({
    fechaActivacion: [new Date().toISOString().substring(0, 10), Validators.required],
  });

  private readonly activar$ = new Subject<void>();

  constructor() {
    this.activar$.pipe(
      exhaustMap(() => {
        const { fechaActivacion } = this.form.getRawValue();
        return this.contratosSvc.activarContrato(this.contratoId(), { fechaActivacion: fechaActivacion! }).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.isSaving.set(false);
            if (!wasHandledByInterceptor(err)) {
              const e = err as { error?: { message?: string; errors?: string[] } };
              const msg = e.error?.message ?? e.error?.errors?.[0] ?? 'Error al activar el contrato';
              this.formErrors.set([msg]);
            }
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.isSaving.set(false);
      if (res.success === false) {
        const r = res as { success: boolean; errors?: string[]; message?: string };
        this.formErrors.set([r.errors?.[0] ?? r.message ?? 'Error al activar el contrato']);
      } else {
        this.utilsSvc.showNotification('Éxito', 'Contrato activado correctamente', 'success');
        this.router.navigate(['/operaciones/contratos-pasivos']);
      }
    });
  }

  ngOnInit(): void {
    this.contratoId.set(+this.route.snapshot.params['id']);
    this.contratoNombre.set(history.state?.contrato ?? '');
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onActivar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.formErrors.set(['La Fecha de Activación es requerida.']);
      return;
    }
    this.formErrors.set([]);
    this.isSaving.set(true);
    this.activar$.next();
  }

  onCancelar(): void {
    this.router.navigate(['/operaciones/contratos-pasivos']);
  }
}
