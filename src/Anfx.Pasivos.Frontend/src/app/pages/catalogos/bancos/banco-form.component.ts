import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { CardComponent } from '../../../shared/components/card/card.component';
import { BancoDto } from '../../../../api/models/models';
import { UtilsService } from '../../../services/utils.service';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';

@Component({
  selector: 'app-banco-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, CardComponent],
  templateUrl: './banco-form.component.html'
})
export class BancoFormComponent implements OnInit {
  private readonly utilsService    = inject(UtilsService);

  isEditMode = signal(false);
  isLoading = signal(false);
  bancoId = signal<number | null>(null);

  bancoForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private catalogosService: CatalogosService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.bancoForm = this.fb.group({
      banco: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.bancoId.set(+id);
      this.loadBanco(this.bancoId()!);
    }
  }

  loadBanco(id: number): void {
    this.isLoading.set(true);
    // Nota: el servicio getBancoById parece tener parámetros innecesarios en su firma
    this.catalogosService.getBancoById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.bancoForm.patchValue(res.data);
        } else {
          this.utilsService.showNotification('Error', 'No se pudo cargar el banco', 'error');
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error de conexión al cargar el banco', 'error');
        }
      }
    });
  }

  isInvalid(field: string): boolean {
    const control = this.bancoForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit(): void {
    if (this.bancoForm.invalid) {
      this.bancoForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const bancoDto: BancoDto = this.bancoForm.value;

    if (this.isEditMode() && this.bancoId()) {
      this.catalogosService.updateBanco(this.bancoId()!, bancoDto).subscribe({
        next: () => this.handleSuccess('Banco actualizado correctamente'),
        error: (err) => this.handleError(err)
      });
    } else {
      this.catalogosService.createBanco(bancoDto).subscribe({
        next: () => this.handleSuccess('Banco creado correctamente'),
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleSuccess(message: string): void {
    this.utilsService.showNotification('Éxito', message, 'success');
    this.router.navigate(['/catalogos/bancos']);
  }

  private handleError(err: any): void {
    this.isLoading.set(false);
    if (!wasHandledByInterceptor(err)) {
      this.utilsService.showNotification('Error', 'Error al procesar la solicitud', 'error');
    }
  }
}
