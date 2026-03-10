import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { BancoDto } from '../../../../api/models/models';

@Component({
  selector: 'app-banco-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './banco-form.component.html'
})
export class BancoFormComponent implements OnInit {
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
    this.catalogosService.getBancoById().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.bancoForm.patchValue(res.data);
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar banco:', err);
        this.isLoading.set(false);
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
    alert(message);
    this.router.navigate(['/catalogos/bancos']);
  }

  private handleError(err: any): void {
    this.isLoading.set(false);
    console.error('Error en la operación:', err);
    alert('Error al procesar la solicitud');
  }
}
