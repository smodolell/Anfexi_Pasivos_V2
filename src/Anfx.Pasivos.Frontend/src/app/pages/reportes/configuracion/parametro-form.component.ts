import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReportesService } from '@api/services/reportes.service';
import { CardComponent } from '@shared/components/card/card.component';
import { ParametroEditDto } from '@api/models/parametroEditDto';
import { UtilsService } from '@services/utils.service';
import { wasHandledByInterceptor } from 'src/app/interceptors/auth.interceptor';

@Component({
  selector: 'app-parametro-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, CardComponent],
  templateUrl: './parametro-form.component.html',
})
export class ParametroFormComponent implements OnInit {
  private readonly reportesService = inject(ReportesService);
  private readonly utilsService = inject(UtilsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  isLoading = signal(false);
  parametroId = signal<string | null>(null);
  reporteId = signal<number | null>(null);

  readonly inputOptions = [
    { value: 1,  label: 'Cuadro de Texto' },
    { value: 2,  label: 'Casilla (Booleano)' },
    { value: 3,  label: 'Fecha' },
    { value: 4,  label: 'Lista Desplegable' },
    { value: 12, label: 'Usuario Activo (automático)' },
  ];

  form: FormGroup = this.fb.group({
    nomParametro:  [{ value: '', disabled: true }],
    display:       ['', [Validators.required, Validators.maxLength(200)]],
    inputId:       [1,  [Validators.required]],
    tipoDato:      [{ value: '', disabled: true }],
    tablaRef:      [''],
    columnaValor:  [''],
    columnaTexto:  [''],
    order:         [0],
  });

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    const rId = this.route.snapshot.params['reporteId'];
    if (id) {
      this.parametroId.set(id);
      this.reporteId.set(rId ? +rId : null);
      this.loadParametro(id);
    }
  }

  get showDropdownFields(): boolean {
    return +this.form.value.inputId === 4;
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const raw = this.form.getRawValue();
    const dto: ParametroEditDto = {
      nomParametro:  raw.nomParametro,
      display:       raw.display,
      inputId:       +raw.inputId,
      tipoDato:      raw.tipoDato || null,
      tablaRef:      raw.tablaRef || null,
      columnaValor:  raw.columnaValor || null,
      columnaTexto:  raw.columnaTexto || null,
      order:         +raw.order,
    };

    this.reportesService.updateParametro(this.parametroId()!, dto).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.utilsService.showNotification('Éxito', 'Parámetro actualizado correctamente', 'success');
        this.router.navigate(['/reportes/configuracion/edit', this.reporteId()]);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al guardar el parámetro', 'error');
        }
      },
    });
  }

  private loadParametro(id: string): void {
    this.isLoading.set(true);
    this.reportesService.getParametroById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.form.patchValue(res.data);
        } else {
          this.utilsService.showNotification('Error', 'No se pudo cargar el parámetro', 'error');
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar el parámetro', 'error');
        }
      },
    });
  }
}
