import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ConfiguracionesService } from 'src/app/core/api/services/configuraciones.service';
import { TipoTablaAmortizaDto } from 'src/app/core/api/models/tipoTablaAmortizaDto';
import { CardComponent } from '@shared/components/card/card.component';

@Component({
  selector: 'app-tipo-tabla-amortiza-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, CardComponent],
  templateUrl: './tipo-tabla-amortiza-form.component.html'
})
export class TipoTablaAmortizaFormComponent implements OnInit {
  private service = inject(ConfiguracionesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  isEditMode = signal(false);
  isLoading = signal(false);
  itemId = signal<number | null>(null);
  errorMsg = signal<string | null>(null);

  form = this.fb.group({
    tipoTablaAmortiza: ['', [Validators.required, Validators.maxLength(200)]],
    esCapitalizable:   [false],
    activo:            [true]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.itemId.set(+id);
      this.loadItem(+id);
    }
  }

  private loadItem(id: number): void {
    this.isLoading.set(true);
    this.service.getTipoTablaAmortizaById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.form.patchValue({
            tipoTablaAmortiza: res.data.tipoTablaAmortiza ?? '',
            esCapitalizable:   res.data.esCapitalizable ?? false,
            activo:            res.data.activo ?? true
          });
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMsg.set('Error al cargar el registro.');
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
    const dto: TipoTablaAmortizaDto = {
      tipoTablaAmortiza: v.tipoTablaAmortiza!,
      esCapitalizable:   v.esCapitalizable ?? false,
      activo:            v.activo ?? true
    };

    if (this.isEditMode() && this.itemId()) {
      this.service.updateTipoTablaAmortiza(this.itemId()!, dto).subscribe({
        next: () => this.router.navigate(['/configuracion/tipo-tabla-amortiza']),
        error: (err) => this.handleError(err)
      });
    } else {
      this.service.createTipoTablaAmortiza(dto).subscribe({
        next: () => this.router.navigate(['/configuracion/tipo-tabla-amortiza']),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    this.isLoading.set(false);
    this.errorMsg.set(err?.error?.message || 'Error al procesar la solicitud.');
  }
}
