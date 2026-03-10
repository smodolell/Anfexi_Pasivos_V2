import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { EstatusContratoDto } from '../../../../api/models/models';

@Component({
  selector: 'app-estatus-contrato-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './estatus-contrato-form.component.html'
})
export class EstatusContratoFormComponent implements OnInit {
  isEditMode = signal(false);
  isLoading = signal(false);
  estatusId = signal<number | null>(null);

  estatusForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private catalogosService: CatalogosService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.estatusForm = this.fb.group({
      estatusContrato: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.estatusId.set(+id);
      this.loadEstatus(this.estatusId()!);
    }
  }

  loadEstatus(id: number): void {
    this.isLoading.set(true);
    this.catalogosService.getEstatusContratoById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.estatusForm.patchValue(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  isInvalid(field: string): boolean {
    const control = this.estatusForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit(): void {
    if (this.estatusForm.invalid) {
      this.estatusForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const dto: EstatusContratoDto = this.estatusForm.value;

    if (this.isEditMode() && this.estatusId()) {
      this.catalogosService.updateEstatusContrato(this.estatusId()!, dto).subscribe({
        next: () => this.handleSuccess('Estatus actualizado correctamente'),
        error: () => this.handleError()
      });
    } else {
      this.catalogosService.createEstatusContrato(dto).subscribe({
        next: () => this.handleSuccess('Estatus creado correctamente'),
        error: () => this.handleError()
      });
    }
  }

  private handleSuccess(message: string): void {
    alert(message);
    this.router.navigate(['/catalogos/estatus-contrato']);
  }

  private handleError(): void {
    this.isLoading.set(false);
    alert('Error al procesar la solicitud');
  }
}
