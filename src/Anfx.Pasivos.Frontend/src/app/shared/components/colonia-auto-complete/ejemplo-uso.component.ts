import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ColoniaAutoCompleteComponent, ColoniaModel } from './colonia-auto-complete.component';

@Component({
  selector: 'app-ejemplo-uso',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ColoniaAutoCompleteComponent],
  template: `
    <div class="card">
      <div class="card-body">
        <h4 class="card-title">Ejemplo de Uso - ColoniaAutoComplete</h4>
        
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="row">
            <div class="col-12">
              <h5>Dirección de la Empresa</h5>
              <app-colonia-auto
              
                [required]="true"
                [codigoPostalRequired]="true"
                labelCodigoPostal="Código Postal"
                labelEstado="Estado"
                labelMunicipio="Municipio"
                labelColonia="Colonia"
                formControlName="direccion">
              </app-colonia-auto>
            </div>
          </div>
          
          <div class="row mt-3">
            <div class="col-12">
              <button type="submit" class="btn btn-primary" [disabled]="form.invalid">
                Guardar
              </button>
            </div>
          </div>
        </form>
        
        <!-- Mostrar datos del modelo -->
        <div class="row mt-4" *ngIf="form.get('direccion')?.value">
          <div class="col-12">
            <h6>Datos del Modelo:</h6>
            <pre>{{ form.get('direccion')?.value | json }}</pre>
          </div>
        </div>
      </div>
    </div>
  `
})
export class EjemploUsoComponent {
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      direccion: [null, Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const direccion: ColoniaModel = this.form.get('direccion')?.value;
      console.log('Dirección seleccionada:', direccion);
      
      // Aquí puedes usar los datos:
      // direccion.codigoPostal
      // direccion.estado
      // direccion.municipio
      // direccion.coloniaId
    }
  }
}
