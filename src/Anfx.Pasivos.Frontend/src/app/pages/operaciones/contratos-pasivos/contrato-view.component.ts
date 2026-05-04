import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { InfoGeneralContratoPasivoDto } from 'src/app/core/api/models/infoGeneralContratoPasivoDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { ContratoAmortizacionPagosComponent } from './contrato-amortizacion-pagos.component';

@Component({
  selector: 'app-contrato-view',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, RouterModule, ContratoAmortizacionPagosComponent],
  templateUrl: './contrato-view.component.html',
})
export class ContratoViewComponent implements OnInit {
  private readonly contratosSvc = inject(ContratosService);
  private readonly utilsSvc     = inject(UtilsService);
  private readonly router       = inject(Router);
  private readonly route        = inject(ActivatedRoute);

  isLoading = signal(true);
  contrato  = signal<InfoGeneralContratoPasivoDto | null>(null);

  ngOnInit(): void {
    const contratoParam = this.route.snapshot.params['contrato'] as string;
    this.contratosSvc.getInfoGeneralContratoPasivo(contratoParam).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          this.contrato.set(res.data);
        } else {
          this.utilsSvc.showNotification('Error', 'No se pudo cargar el contrato', 'error');
          this.router.navigate(['/operaciones/contratos-pasivos']);
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsSvc.showNotification('Error', 'Error al cargar el contrato', 'error');
        }
        this.router.navigate(['/operaciones/contratos-pasivos']);
      },
    });
  }

  onVolver(): void {
    this.router.navigate(['/operaciones/contratos-pasivos']);
  }

  fmt(val?: string | null): string {
    if (!val) return '—';
    return val.substring(0, 10);
  }
}
