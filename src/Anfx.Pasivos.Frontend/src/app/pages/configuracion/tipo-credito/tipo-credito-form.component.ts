import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ConfiguracionesService } from 'src/app/core/api/services/configuraciones.service';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { TipoCreditoDto } from 'src/app/core/api/models/tipoCreditoDto';
import { TipoTablaAmortizaListItemDto } from 'src/app/core/api/models/tipoTablaAmortizaListItemDto';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { CardComponent } from '@shared/components/card/card.component';

@Component({
  selector: 'app-tipo-credito-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, CardComponent],
  templateUrl: './tipo-credito-form.component.html'
})
export class TipoCreditoFormComponent implements OnInit {
  private service = inject(ConfiguracionesService);
  private selectListsService = inject(SelectListsService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  isEditMode = signal(false);
  isLoading = signal(false);
  tipoCreditoId = signal<number | null>(null);
  errorMsg = signal<string | null>(null);
  tiposTablaAmortiza = signal<TipoTablaAmortizaListItemDto[]>([]);
  tiposMovimiento = signal<SelectItemDto[]>([]);

  form = this.fb.group({
    tipoCredito:         ['', [Validators.required, Validators.maxLength(200)]],
    prefijo:             ['', [Validators.required, Validators.maxLength(10)]],
    sufijo:              ['', [Validators.required, Validators.maxLength(10)]],
    contador:            [0],
    idTipoTablaAmortiza: [null as number | null, Validators.required],
    idTipoMovimiento:    [null as number | null, Validators.required],
    idTipoMovimiento_Mora: [null as number | null, Validators.required],
    activo:              [true]
  });

  ngOnInit(): void {
    this.loadTiposTablaAmortiza();
    this.loadTiposMovimiento();

    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.tipoCreditoId.set(+id);
      this.loadTipoCredito(+id);
    }
  }

  private loadTiposMovimiento(): void {
    this.selectListsService.getTipoMovimientos().subscribe({
      next: (res) => this.tiposMovimiento.set(res.data ?? [])
    });
  }

  private loadTiposTablaAmortiza(): void {
    this.service.apiConfiguracionesTipoTablaAmortizaGet(undefined, 1, 100).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.tiposTablaAmortiza.set(res.data.results || []);
        }
      }
    });
  }

  private loadTipoCredito(id: number): void {
    this.isLoading.set(true);
    this.service.getTipoCreditoById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const d = res.data;
          this.form.patchValue({
            tipoCredito:           d.tipoCredito ?? '',
            prefijo:               d.prefijo ?? '',
            sufijo:                d.sufijo ?? '',
            contador:              d.contador ?? 0,
            idTipoTablaAmortiza:   d.idTipoTablaAmortiza ?? null,
            idTipoMovimiento:      d.idTipoMovimiento ?? null,
            idTipoMovimiento_Mora: d.idTipoMovimiento_Mora ?? null,
            activo:                d.activo ?? true
          });
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMsg.set('Error al cargar el tipo de crédito.');
        this.isLoading.set(false);
      }
    });
  }

  isInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMsg.set(null);

    const v = this.form.value;
    const dto: TipoCreditoDto = {
      tipoCredito:           v.tipoCredito!,
      prefijo:               v.prefijo!,
      sufijo:                v.sufijo!,
      contador:              v.contador ?? 0,
      idTipoTablaAmortiza:   v.idTipoTablaAmortiza!,
      idTipoMovimiento:      v.idTipoMovimiento!,
      idTipoMovimiento_Mora: v.idTipoMovimiento_Mora!,
      activo:                v.activo ?? true
    };

    if (this.isEditMode() && this.tipoCreditoId()) {
      this.service.updateTipoCredito(this.tipoCreditoId()!, dto).subscribe({
        next: () => this.router.navigate(['/configuracion/tipo-credito']),
        error: (err) => this.handleError(err)
      });
    } else {
      this.service.createTipoCredito(dto).subscribe({
        next: () => this.router.navigate(['/configuracion/tipo-credito']),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    this.isLoading.set(false);
    this.errorMsg.set(err?.error?.message || 'Error al procesar la solicitud.');
  }
}
