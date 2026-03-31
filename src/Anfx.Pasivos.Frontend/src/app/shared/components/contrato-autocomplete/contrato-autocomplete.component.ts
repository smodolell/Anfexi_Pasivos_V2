import {
  Component,
  OnDestroy,
  OnInit,
  forwardRef,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs';
import { AutocompleteResultDto } from 'src/app/core/api/models/autocompleteResultDto';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { SearchInputComponent } from '../search-input/search-input.component';

@Component({
  selector: 'app-contrato-autocomplete',
  standalone: true,
  imports: [FormsModule, SearchInputComponent],
  templateUrl: './contrato-autocomplete.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContratoAutocompleteComponent),
      multi: true,
    },
  ],
})
export class ContratoAutocompleteComponent implements OnInit, OnDestroy, ControlValueAccessor {
  private readonly contratosSvc = inject(ContratosService);
  private readonly destroy$ = new Subject<void>();
  private readonly search$ = new Subject<string>();

  // ── Inputs ───────────────────────────────────────────────────
  label = input<string>('Número de Contrato');
  placeholder = input<string>('Ingrese el número de contrato...');
  required = input<boolean>(false);
  disabled = input<boolean>(false);
  searchChanged   = output<string>();
  // ── Output ───────────────────────────────────────────────────
  /** Emite el item seleccionado del dropdown */
  contratoSelected = output<AutocompleteResultDto>();

  // ── Estado interno ───────────────────────────────────────────
  inputValue = signal<string>('');
  sugerencias = signal<AutocompleteResultDto[]>([]);
  loading = signal<boolean>(false);
  showDropdown = signal<boolean>(false);
  isDisabled = signal<boolean>(false);

  // ── ControlValueAccessor ─────────────────────────────────────
  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  ngOnInit(): void {
    this.search$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((term) => {
          if (!term || term.length < 2) {
            this.sugerencias.set([]);
            this.showDropdown.set(false);
            this.loading.set(false);
            return [];
          }
          this.loading.set(true);
          return this.contratosSvc.getAutocompleteContrato(term);
        }),
        takeUntil(this.destroy$),
      )
      .subscribe({
        next: (res) => {
          this.loading.set(false);
          this.sugerencias.set(res.data ?? []);
          this.showDropdown.set((res.data?.length ?? 0) > 0);
        },
        error: () => {
          this.loading.set(false);
          this.sugerencias.set([]);
          this.showDropdown.set(false);
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ── Handlers del template ─────────────────────────────────────
  onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.inputValue.set(value);

    if (!value) {
      this.onChange(null);
    }

    this.search$.next(value);
  }

  onSelect(item: AutocompleteResultDto): void {
    console.log('contato seleccionado', item);
    this.inputValue.set(item.label ?? '');
    this.showDropdown.set(false);
    this.sugerencias.set([]);
    this.onChange(item.value ?? null);
    this.contratoSelected.emit(item);
  }

  onEnter(): void {
    const text = this.inputValue().trim();
    if (!text) return;
    this.showDropdown.set(false);
    this.sugerencias.set([]);
    this.onChange(text);
    this.contratoSelected.emit({ value: text, label: text });
  }

  onBlur(): void {
    // Delay para permitir el click en el dropdown antes de ocultarlo
    setTimeout(() => {
      this.showDropdown.set(false);
      this.onTouched();
    }, 200);
  }

  onFocus(): void {
    if (this.sugerencias().length > 0) {
      this.showDropdown.set(true);
    }
  }

  clear(): void {
    this.inputValue.set('');
    this.sugerencias.set([]);
    this.showDropdown.set(false);
    this.onChange(null);
    this.searchChanged.emit('');
  }

  // ── ControlValueAccessor ──────────────────────────────────────
  writeValue(value: string | null): void {
    this.inputValue.set(value ?? '');
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }
}
