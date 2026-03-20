import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { CuentaBancariaDto, BancoListItemDto } from '../../../../api/models/models';
import { UtilsService } from '../../../services/utils.service';

@Component({
  selector: 'app-cuenta-bancaria-form',
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './cuenta-bancaria-form.component.html'
})
export class CuentaBancariaFormComponent implements OnInit {
  isEditMode = signal(false);
  isLoading = signal(false);
  cuentaId = signal<number | null>(null);
  bancos = signal<BancoListItemDto[]>([]);

  cuentaForm: FormGroup;

  private readonly utilsService = inject(UtilsService);

  constructor(
    private fb: FormBuilder,
    private catalogosService: CatalogosService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.cuentaForm = this.fb.group({
      idBanco: [null, [Validators.required]],
      cuentaBancaria: ['', [Validators.required, Validators.maxLength(20)]],
      clabe: ['', [Validators.required, Validators.minLength(18), Validators.maxLength(18)]]
    });
  }

  ngOnInit(): void {
    this.loadBancos();
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.isEditMode.set(true);
      this.cuentaId.set(+id);
      this.loadCuenta(this.cuentaId()!);
    }
  }

  loadBancos(): void {
    // Obtenemos una lista de bancos para el selector (simplificado para el ejemplo)
    this.catalogosService.apiCatalogosBancoGet('', 1, 100).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.bancos.set(res.data.results || []);
        }
      }
    });
  }

  loadCuenta(id: number): void {
    this.isLoading.set(true);
    this.catalogosService.getCuentaBancariaById(id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.cuentaForm.patchValue(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  isInvalid(field: string): boolean {
    const control = this.cuentaForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit(): void {
    if (this.cuentaForm.invalid) {
      this.cuentaForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const dto: CuentaBancariaDto = this.cuentaForm.value;

    if (this.isEditMode() && this.cuentaId()) {
      this.catalogosService.updateCuentaBancaria(this.cuentaId()!, dto).subscribe({
        next: () => this.handleSuccess('Cuenta bancaria actualizada correctamente'),
        error: () => this.handleError()
      });
    } else {
      this.catalogosService.createCuentaBancaria(dto).subscribe({
        next: () => this.handleSuccess('Cuenta bancaria creada correctamente'),
        error: () => this.handleError()
      });
    }
  }

  private handleSuccess(message: string): void {
    this.isLoading.set(false);
    this.utilsService.showNotification('Éxito', message, 'success');
    this.router.navigate(['/catalogos/cuentas-bancarias']);
  }

  private handleError(): void {
    this.isLoading.set(false);
    this.utilsService.showNotification('Error', 'Error al procesar la solicitud', 'error');
  }
}
