import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ConfiguracionesService } from '../../../../api/services/configuraciones.service';
import { FondeadorEditDto } from '../../../../api/models/fondeadorEditDto';

@Component({
  selector: 'app-fondeador-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './fondeador-form.component.html'
})
export class FondeadorFormComponent implements OnInit {
  private service = inject(ConfiguracionesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  isEditMode = signal(false);
  isLoading = signal(false);
  fondeadorId = signal<number | null>(null);
  errorMsg = signal<string | null>(null);

  form = this.fb.group({
    fondeador: ['', [Validators.required, Validators.maxLength(200)]],
    claveCuentaContable: ['', Validators.maxLength(50)]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.fondeadorId.set(+id);
      this.loadFondeador(+id);
    }
  }

  private loadFondeador(id: number): void {
    this.isLoading.set(true);
    this.service.getFondeadorById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.form.patchValue({
            fondeador: res.data.fondeador ?? '',
            claveCuentaContable: res.data.claveCuentaContable ?? ''
          });
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMsg.set('Error al cargar el fondeador.');
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

    const dto: FondeadorEditDto = {
      fondeador: this.form.value.fondeador!,
      claveCuentaContable: this.form.value.claveCuentaContable || null
    };

    if (this.isEditMode() && this.fondeadorId()) {
      this.service.updateFondeador(this.fondeadorId()!, dto).subscribe({
        next: () => this.router.navigate(['/configuracion/fondeador']),
        error: (err) => this.handleError(err)
      });
    } else {
      this.service.createFondeador(dto).subscribe({
        next: () => this.router.navigate(['/configuracion/fondeador']),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    this.isLoading.set(false);
    this.errorMsg.set(err?.error?.message || 'Error al procesar la solicitud.');
  }
}
