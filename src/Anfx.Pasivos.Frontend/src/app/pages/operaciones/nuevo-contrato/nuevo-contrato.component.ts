import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';

@Component({
  selector: 'app-nuevo-contrato',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './nuevo-contrato.component.html',
})
export class NuevoContratoComponent implements OnInit {
  private readonly selectSvc    = inject(SelectListsService);
  private readonly contratosSvc = inject(ContratosService);
  private readonly utilsSvc     = inject(UtilsService);
  private readonly router       = inject(Router);
  private readonly fb           = inject(FormBuilder);

  fondeadores        = signal<SelectItemDto[]>([]);
  lineasCredito      = signal<SelectItemDto[]>([]);
  loadingFondeadores = signal(false);
  loadingLineas      = signal(false);
  loadingForm        = signal(false);

  form = this.fb.group({
    idFondeador:    [null as number | null, Validators.required],
    idLineaCredito: [null as number | null, Validators.required],
  });

  ngOnInit(): void {
    this.cargarFondeadores();

    this.form.get('idFondeador')!.valueChanges.subscribe(idFondeador => {
      this.form.patchValue({ idLineaCredito: null }, { emitEvent: false });
      this.lineasCredito.set([]);
      if (idFondeador) {
        this.cargarLineasCredito(idFondeador);
      }
    });
  }

  onNuevoContrato(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const idLineaCredito = this.form.getRawValue().idLineaCredito!;
    this.loadingForm.set(true);

    this.contratosSvc.getAddContrato(idLineaCredito).subscribe({
      next: (res) => {
        this.loadingForm.set(false);
        if (res.success && res.data) {
          const lineaSeleccionada = this.lineasCredito().find(
            l => l.value === this.form.getRawValue().idLineaCredito
          );
          this.router.navigate(['/operaciones/contratos-pasivos/nuevo'], {
            state: {
              editDto: res.data,
              lineaCreditoNombre: lineaSeleccionada?.text ?? '',
            },
          });
        } else {
          const msg = res.errors?.[0] ?? res.message ?? 'Error al preparar el contrato';
          this.utilsSvc.showNotification('Error', msg, 'error');
        }
      },
      error: (err) => {
        this.loadingForm.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error de conexión al preparar el contrato', 'error');
        }
      },
    });
  }

  private cargarFondeadores(): void {
    this.loadingFondeadores.set(true);
    this.selectSvc.getFondeadoresSelectList().subscribe({
      next: (res) => { this.fondeadores.set(res.data ?? []); this.loadingFondeadores.set(false); },
      error: (err) => {
        this.loadingFondeadores.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar fondeadores', 'error');
        }
      },
    });
  }

  private cargarLineasCredito(idFondeador: number): void {
    this.loadingLineas.set(true);
    this.selectSvc.getLineasCreditoByFondeador(idFondeador).subscribe({
      next: (res) => { this.lineasCredito.set(res.data ?? []); this.loadingLineas.set(false); },
      error: (err) => {
        this.loadingLineas.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar líneas de crédito', 'error');
        }
      },
    });
  }
}
