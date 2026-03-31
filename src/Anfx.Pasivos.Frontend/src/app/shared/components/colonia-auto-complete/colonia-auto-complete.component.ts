import { ChangeDetectionStrategy, Component, DestroyRef, Input, OnInit, forwardRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';

import { ColoniaService } from '@services/catalogos/colonia.service';
import { UtilsService } from '@services/utils.service';
import { SelectItemDto } from '../../../../types/selectitem.dto';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

export interface ColoniaModel {
  codigoPostal: string;
  estado: string;
  municipio: string;
  coloniaId: number | null;
}

@Component({
  selector: 'app-colonia-auto',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule],
  templateUrl: './colonia-auto-complete.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ColoniaAutoCompleteComponent),
      multi: true
    }
  ]
})
export class ColoniaAutoCompleteComponent implements OnInit, ControlValueAccessor {
  @Input() required = false;
  @Input() codigoPostalRequired = false;
  @Input() labelCodigoPostal = 'Código Postal';
  @Input() labelEstado = 'Estado';
  @Input() labelMunicipio = 'Municipio';
  @Input() labelColonia = 'Colonia';

  private readonly coloniaService          = inject(ColoniaService);
  private readonly utilsService            = inject(UtilsService);
  private readonly destroyRef              = inject(DestroyRef);
  private readonly codigoPostalDebounce$   = new Subject<string>();

  // Signals para el estado del componente
  codigoPostal = signal<string>('');
  estado = signal<string>('');
  municipio = signal<string>('');
  coloniaId = signal<number | null>(null);
  colonias = signal<SelectItemDto[]>([]);
  codigosPostales = signal<SelectItemDto[]>([]);

  // Estados de carga
  loadingCodigosPostales = signal<boolean>(false);
  loadingColonias = signal<boolean>(false);
  loadingById = signal<boolean>(false);

  // Estados de autocompletado
  showCodigosPostales = signal<boolean>(false);
  showColonias = signal<boolean>(false);

  // ControlValueAccessor
  private onChange = (_value: ColoniaModel) => {};
  private onTouched = () => {};

  ngOnInit(): void {
    this.setupCodigoPostalAutocomplete();
    this.setupCodigoPostalDebounce();
  }

  private setupCodigoPostalAutocomplete(): void {
    // Autocompletado para código postal - usar el signal directamente
    // No necesitamos un FormControl separado, usamos el signal codigoPostal
    // y lo conectamos con el método onCodigoPostalInput
  }

  private setupCodigoPostalDebounce(): void {
    this.codigoPostalDebounce$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(value => {
        if (value && value.length >= 3) {
          this.loadingCodigosPostales.set(true);
          this.showCodigosPostales.set(true);

          this.coloniaService.getCodigosPostales(value)
            .subscribe({
              next: (response) => {
                this.loadingCodigosPostales.set(false);
                if (response && response.success) {
                  this.codigosPostales.set(response.data);
                } else {
                  this.codigosPostales.set([]);
                }
              },
              error: () => {
                this.loadingCodigosPostales.set(false);
              }
            });
        } else {
          this.codigosPostales.set([]);
          this.showCodigosPostales.set(false);
        }
      });
  }

  onCodigoPostalInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.codigoPostal.set(value);
    this.updateModel();

    // Emitir valor para el debounce
    this.codigoPostalDebounce$.next(value);
  }

  onCodigoPostalSelect(codigoPostal: SelectItemDto): void {
    this.codigoPostal.set(codigoPostal.text);
    this.showCodigosPostales.set(false);
    this.loadColoniasByCodigoPostal(codigoPostal.text);
  }

  onCodigoPostalBlur(): void {
    setTimeout(() => {
      this.showCodigosPostales.set(false);
    }, 200);
  }

  onColoniaSelect(event: Event): void {
    const coloniaId = parseInt((event.target as HTMLSelectElement).value);
    this.coloniaId.set(coloniaId);
    this.updateModel();
  }

  private loadColoniasByCodigoPostal(codigoPostal: string): void {
    this.loadingColonias.set(true);
    this.coloniaService.getColoniasByCodigoPostal(codigoPostal).subscribe({
      next: (response) => {
        this.loadingColonias.set(false);
        if (response.success) {
          this.estado.set(response.data.estado);
          this.municipio.set(response.data.municipio);
          this.colonias.set(response.data.colonias);
          this.showColonias.set(true);
          this.updateModel();
        } else {
          this.utilsService.showNotification('Error', 'Error al cargar colonias', 'error');
        }
      },
      error: () => {
        this.loadingColonias.set(false);
      }
    });
  }

  private loadColoniasById(id: number): void {
    this.loadingById.set(true);
    this.coloniaService.getColoniasById(id).subscribe({
      next: (response) => {
        this.loadingById.set(false);
        if (response.success) {
          this.codigoPostal.set(response.data.codigoPostal);
          this.estado.set(response.data.estado);
          this.municipio.set(response.data.municipio);
          this.colonias.set(response.data.colonias);
          this.coloniaId.set(id);
          this.updateModel();
        } else {
          this.utilsService.showNotification('Error', 'Error al cargar datos de colonia', 'error');
        }
      },
      error: () => {
        this.loadingById.set(false);
      }
    });
  }

  private updateModel(): void {
    const model: ColoniaModel = {
      codigoPostal: this.codigoPostal(),
      estado: this.estado(),
      municipio: this.municipio(),
      coloniaId: this.coloniaId()
    };
    this.onChange(model);
  }

  // ControlValueAccessor implementation
  writeValue(value: ColoniaModel | null): void {
    if (value) {
      if (value.coloniaId) {
        // Si tiene coloniaId, cargar datos por ID
        this.loadColoniasById(value.coloniaId);
      } else {
        // Si solo tiene código postal, cargar colonias
        this.codigoPostal.set(value.codigoPostal);
        this.estado.set(value.estado);
        this.municipio.set(value.municipio);
        this.coloniaId.set(value.coloniaId);
        if (value.codigoPostal) {
          this.loadColoniasByCodigoPostal(value.codigoPostal);
        }
      }
    } else {
      this.resetForm();
    }
  }

  registerOnChange(fn: (value: ColoniaModel) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(_isDisabled: boolean): void {
    // Implementar si es necesario
  }

  private resetForm(): void {
    this.codigoPostal.set('');
    this.estado.set('');
    this.municipio.set('');
    this.coloniaId.set(null);
    this.colonias.set([]);
    this.codigosPostales.set([]);
    this.showCodigosPostales.set(false);
    this.showColonias.set(false);
  }

  // Métodos para el template
  onFocusCodigoPostal(): void {
    if (this.codigosPostales().length > 0) {
      this.showCodigosPostales.set(true);
    }
  }

  onFocusColonia(): void {
    if (this.colonias().length > 0) {
      this.showColonias.set(true);
    }
  }

  onBlurColonia(): void {
    setTimeout(() => {
      this.showColonias.set(false);
    }, 200);
  }
}
