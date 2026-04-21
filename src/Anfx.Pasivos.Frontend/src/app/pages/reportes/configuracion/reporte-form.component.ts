import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReportesService } from '@api/services/reportes.service';
import { CardComponent } from '@shared/components/card/card.component';
import { ReporteEditDto } from '@api/models/reporteEditDto';
import { ParametroListItemDto } from '@api/models/parametroListItemDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';

@Component({
  selector: 'app-reporte-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, CardComponent],
  templateUrl: './reporte-form.component.html',
})
export class ReporteFormComponent implements OnInit {
  private readonly reportesService = inject(ReportesService);
  private readonly utilsService = inject(UtilsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  isEditMode = signal(false);
  isLoading = signal(false);
  isLoadingParams = signal(false);
  reporteId = signal<number | null>(null);
  parametros = signal<ParametroListItemDto[]>([]);

  readonly formatoOptions = [
    { value: 1, label: 'Excel' },
    { value: 2, label: 'Texto' },
  ];

  form: FormGroup = this.fb.group({
    nomReporte:      ['', [Validators.required, Validators.maxLength(200)]],
    storedProcedure: ['', [Validators.required, Validators.maxLength(200)]],
    reporteFormatoId: [1, [Validators.required]],
    activo:          [true],
  });

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.reporteId.set(+id);
      this.loadReporte(+id);
      this.loadParametros(+id);
    }
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onEditParametro(param: ParametroListItemDto) {
    this.router.navigate(['/reportes/configuracion', this.reporteId(), 'parametros', param.id]);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const dto: ReporteEditDto = {
      nomReporte:       this.form.value.nomReporte,
      storedProcedure:  this.form.value.storedProcedure,
      reporteFormatoId: +this.form.value.reporteFormatoId,
      activo:           this.form.value.activo,
    };

    if (this.isEditMode() && this.reporteId()) {
      this.reportesService.updateReporte(this.reporteId()!, dto).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.utilsService.showNotification('Éxito', 'Reporte actualizado correctamente', 'success');
          this.loadParametros(this.reporteId()!);
        },
        error: (err) => this.handleError(err),
      });
    } else {
      this.reportesService.createReporte(dto).subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.success && res.data) {
            this.utilsService.showNotification('Éxito', 'Reporte creado correctamente', 'success');
            this.router.navigate(['/reportes/configuracion/edit', res.data]);
          } else {
            this.utilsService.showNotification('Error', res.message ?? 'Error al crear el reporte', 'error');
          }
        },
        error: (err) => this.handleError(err),
      });
    }
  }

  private loadReporte(id: number): void {
    this.isLoading.set(true);
    this.reportesService.getReporteById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.form.patchValue(res.data);
        } else {
          this.utilsService.showNotification('Error', 'No se pudo cargar el reporte', 'error');
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar el reporte', 'error');
        }
      },
    });
  }

  private loadParametros(reporteId: number): void {
    this.isLoadingParams.set(true);
    this.reportesService.apiReportesParametroGet(reporteId).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.parametros.set(res.data);
        }
        this.isLoadingParams.set(false);
      },
      error: (err) => {
        this.isLoadingParams.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar parámetros', 'error');
        }
      },
    });
  }

  private handleError(err: any): void {
    this.isLoading.set(false);
    if (!wasHandledByInterceptor(err)) {
      this.utilsService.showNotification('Error', 'Error al procesar la solicitud', 'error');
    }
  }
}
