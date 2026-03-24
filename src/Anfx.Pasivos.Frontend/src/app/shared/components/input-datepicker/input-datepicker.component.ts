import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, EventEmitter, HostListener, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, forwardRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ReactiveFormsModule, FormControl, ControlContainer, Validators } from '@angular/forms';

import { Subject } from 'rxjs';

@Component({
  selector: 'app-input-datepicker',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  templateUrl: './input-datepicker.component.html',
  styleUrls: ['./input-datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputDatepickerComponent),
      multi: true
    }
  ]
})
export class InputDatepickerComponent implements ControlValueAccessor, OnInit, OnChanges {
  private elementRef = inject(ElementRef);
  private controlContainer = inject(ControlContainer, { optional: true });

  @ViewChild('dateInput', { static: false }) dateInput!: ElementRef<HTMLInputElement>;
  
  @Input() placeholder = 'dd/mm/aaaa';
  @Input() required = false;
  @Input() readonly = false;
  @Input() disabled = false;
  @Input() minDate?: Date;
  @Input() maxDate?: Date;
  @Input() formControlName = '';
  @Input() showFooter = false;
  
  @Output() dateChange = new EventEmitter<Date | null>();

  isOpen = signal(false);
  selectedDate = signal<Date | null>(null);
  currentDate = signal(new Date());
  displayDate = signal(new Date());
  inputValue = signal('');
  dropdownStyle = signal<{ top: string; left: string }>({ top: '0px', left: '0px' });
  formControl: FormControl | null = null;
  private readonly destroyRef = inject(DestroyRef);
  private readonly destroy$   = new Subject<void>();
  
  // Días de la semana en español
  weekDays = ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'];
  months = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];

  private onChange = (_value: Date | null) => {};
  private onTouched = () => {};

  ngOnInit(): void {
    this.displayDate.set(new Date());
    if (this.formControlName && this.controlContainer) {
      this.formControl = this.controlContainer.control?.get(this.formControlName) as FormControl;
      
      if (this.formControl) {
        // Sincronizar el valor inicial PRIMERO
        if (this.formControl.value) {
          const date = new Date(this.formControl.value);
          this.selectedDate.set(date);
          this.inputValue.set(this.formatDateForInput(date));
        }
        
        // Aplicar validaciones después de tener el valor sincronizado
        this.applyValidators();
        
        // Aplicar estado deshabilitado inicial
        if (this.disabled) {
          this.formControl.disable();
        }
        
        // Suscribirse a cambios del FormControl
        this.formControl.valueChanges
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe(value => {
            if (value) {
              const date = new Date(value);
              this.selectedDate.set(date);
              this.inputValue.set(this.formatDateForInput(date));
            } else {
              this.selectedDate.set(null);
              this.inputValue.set('');
            }
          });
      }
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Reaplicar validaciones si cambian los parámetros de validación
    if (this.formControl && (changes['required'] || changes['minDate'] || changes['maxDate'])) {
      this.applyValidators();
    }
    
    // Manejar cambio de estado deshabilitado
    if (changes['disabled']) {
      if (this.formControl) {
        if (this.disabled) {
          this.formControl.disable();
        } else {
          this.formControl.enable();
        }
      }
    }
  }

  // ControlValueAccessor implementation
  writeValue(value: Date | null): void {
    this.selectedDate.set(value);
    if (value) {
      this.displayDate.set(new Date(value));
      this.inputValue.set(this.formatDateForInput(value));
    } else {
      // NUNCA limpiar el input desde writeValue cuando hay contenido
      // Solo limpiar si el input está realmente vacío
      const currentValue = this.inputValue();
      if (!currentValue || currentValue.trim() === '') {
        this.inputValue.set('');
      }
    }
  }

  registerOnChange(fn: (value: Date | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  private applyValidators(): void {
    if (!this.formControl) return;

    const validators = [];

    if (this.required) {
      validators.push(Validators.required);
    }

    this.formControl.markAsUntouched();
    this.formControl.markAsPristine();
    this.formControl.setValidators(validators);
  }

  get isFieldInvalid(): boolean {
    if (!this.formControl) return false;
    
    // Si está touched y tiene errores del FormControl
    if (this.formControl.touched && this.formControl.invalid) {
      return true;
    }
    
    // Verificar si hay fecha inválida localmente
    if (this.formControl.touched) {
      const value = this.inputValue();
      if (value && value.trim() !== '') {
        const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
        const match = value.match(dateRegex);
        
        if (match) {
          const day = parseInt(match[1], 10);
          const month = parseInt(match[2], 10) - 1;
          const year = parseInt(match[3], 10);
          
          if (this.isValidDate(day, month + 1, year)) {
            const date = new Date(year, month, day);
            if (date.getDate() !== day || date.getMonth() !== month || date.getFullYear() !== year) {
              return true; // Fecha inválida
            }
          } else {
            return true; // Fecha inválida
          }
        } else {
          return true; // Formato inválido
        }
      }
    }
    
    return false;
  }

  getFieldError(): string {
    // Solo mostrar errores si está touched (después de onBlur)
    if (!this.formControl?.touched) return '';
    
    const value = this.inputValue();
    
    // 1. Verificar si es requerido y está vacío
    if (this.required && (!value || value.trim() === '')) {
      return 'Este campo es requerido';
    }
    
    // 2. Si tiene valor, verificar si es una fecha válida
    if (value && value.trim() !== '') {
      const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
      const match = value.match(dateRegex);
      
      if (match) {
        const day = parseInt(match[1], 10);
        const month = parseInt(match[2], 10) - 1;
        const year = parseInt(match[3], 10);
        
        if (this.isValidDate(day, month + 1, year)) {
          const date = new Date(year, month, day);
          if (date.getDate() !== day || date.getMonth() !== month || date.getFullYear() !== year) {
            return 'Fecha inválida';
          }
        } else {
          return 'Fecha inválida';
        }
      } else {
        // Si no tiene formato completo dd/mm/aaaa, es fecha inválida
        return 'Fecha inválida';
      }
    }
    
    return '';
  }

  toggleCalendar(): void {
    if (this.disabled || this.readonly) return;

    this.isOpen.set(!this.isOpen());
    if (this.isOpen()) {
      this.onTouched();
      this.updateDropdownPosition();
      // Agregar listener global para navegación del calendario
      document.addEventListener('keydown', this.handleGlobalKeyDown.bind(this));
    } else {
      // Remover listener global cuando se cierra
      document.removeEventListener('keydown', this.handleGlobalKeyDown.bind(this));
    }
  }

  private updateDropdownPosition(): void {
    const inputEl = this.dateInput?.nativeElement || this.elementRef.nativeElement;
    const rect = inputEl.getBoundingClientRect();
    this.dropdownStyle.set({
      top: `${rect.bottom + 4}px`,
      left: `${rect.left}px`
    });
  }

  closeCalendar(): void {
    this.isOpen.set(false);
    // Remover listener global cuando se cierra
    document.removeEventListener('keydown', this.handleGlobalKeyDown.bind(this));
  }

  handleGlobalKeyDown(event: KeyboardEvent): void {
    if (!this.isOpen()) return;
    const target = event.target as HTMLElement;
    if (target.tagName === 'INPUT') return; // Ignore if input has focus
    
    if (event.key === 'ArrowLeft') {
      event.preventDefault();
      this.navigateMonth('prev');
    } else if (event.key === 'ArrowRight') {
      event.preventDefault();
      this.navigateMonth('next');
    } else if (event.key === 'Escape') {
      event.preventDefault();
      this.closeCalendar();
    }
  }

  selectDate(date: Date): void {
    const selectedDate = new Date(date);
    this.selectedDate.set(selectedDate);
    this.displayDate.set(new Date(date));
    this.inputValue.set(this.formatDateForInput(selectedDate));
    this.onChange(selectedDate);
    this.dateChange.emit(selectedDate);
    
    // Actualizar el FormControl si existe
    if (this.formControl) {
      this.formControl.setValue(selectedDate);
    }
    
    this.closeCalendar();
  }

  clearDate(): void {
    this.selectedDate.set(null);
    this.inputValue.set('');
    this.onChange(null);
    this.dateChange.emit(null);
    
    // Actualizar el FormControl si existe
    if (this.formControl) {
      this.formControl.setValue(null);
    }
    
    this.closeCalendar();
  }

  selectToday(): void {
    const today = new Date();
    this.selectDate(today);
  }

  navigateMonth(direction: 'prev' | 'next'): void {
    const currentDate = this.displayDate();
    if (direction === 'prev') {
      currentDate.setMonth(currentDate.getMonth() - 1);
    } else {
      currentDate.setMonth(currentDate.getMonth() + 1);
    }
    this.displayDate.set(new Date(currentDate));
  }

  getCalendarDays(): Date[] {
    const displayDate = this.displayDate();
    const year = displayDate.getFullYear();
    const month = displayDate.getMonth();
    
    const firstDay = new Date(year, month, 1);
    const _lastDay = new Date(year, month + 1, 0);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay());
    
    const days: Date[] = [];
    const currentDate = new Date(startDate);
    
    // Generar 42 días (6 semanas)
    for (let i = 0; i < 42; i++) {
      days.push(new Date(currentDate));
      currentDate.setDate(currentDate.getDate() + 1);
    }
    
    return days;
  }

  isCurrentMonth(date: Date): boolean {
    return date.getMonth() === this.displayDate().getMonth();
  }

  isSelected(date: Date): boolean {
    const selected = this.selectedDate();
    if (!selected) return false;
    return date.toDateString() === selected.toDateString();
  }

  isToday(date: Date): boolean {
    const today = new Date();
    return date.toDateString() === today.toDateString();
  }

  isDisabled(date: Date): boolean {
    if (this.minDate && date < this.minDate) return true;
    if (this.maxDate && date > this.maxDate) return true;
    return false;
  }

  formatDateForInput(date: Date): string {
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }

  getMonthYearText(): string {
    const displayDate = this.displayDate();
    return `${this.months[displayDate.getMonth()]} ${displayDate.getFullYear()}`;
  }

  onInputChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    let value = target.value;
    
    // Solo números
    const numbers = value.replace(/\D/g, '');
    
    // Máximo 8 dígitos
    if (numbers.length > 8) {
      value = numbers.substring(0, 8);
    } else {
      value = numbers;
    }
    
    // Aplicar máscara automática
    value = this.applyAutoMask(value);
    
    this.inputValue.set(value);
    target.value = value;
  }

  private applyAutoMask(numbers: string): string {
    if (numbers.length === 0) return '';
    if (numbers.length <= 2) return numbers;
    if (numbers.length <= 4) return numbers.substring(0, 2) + '/' + numbers.substring(2);
    if (numbers.length <= 8) return numbers.substring(0, 2) + '/' + numbers.substring(2, 4) + '/' + numbers.substring(4);
    return numbers.substring(0, 2) + '/' + numbers.substring(2, 4) + '/' + numbers.substring(4, 8);
  }


  private isValidDate(day: number, month: number, year: number): boolean {
    // Validaciones básicas
    if (year < 1900 || year > 2100) return false;
    if (month < 1 || month > 12) return false;
    if (day < 1 || day > 31) return false;
    
    // Validar días por mes
    const daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    
    // Año bisiesto
    if (month === 2 && this.isLeapYear(year)) {
      daysInMonth[1] = 29;
    }
    
    return day <= daysInMonth[month - 1];
  }

  private isLeapYear(year: number): boolean {
    return (year % 4 === 0 && year % 100 !== 0) || (year % 400 === 0);
  }

  onKeyDown(event: KeyboardEvent): void {
    const allowedKeys = ['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
    const isDigit = event.key >= '0' && event.key <= '9';
    
    if (!isDigit && !allowedKeys.includes(event.key)) {
      event.preventDefault();
    }
  }

  onInputFocus(): void {
    this.onTouched();
  }

  onInputBlur(): void {
    this.onTouched();
    
    const value = this.inputValue();
    let isValid = true;
    let errorType = '';
    
    // 1. Validar si es vacío y es requerido
    if (this.required && (!value || value.trim() === '')) {
      isValid = false;
      errorType = 'required';
    }
    
    // 2. Si no está vacío, validar si es una fecha correcta
    if (isValid && value && value.trim() !== '') {
      const dateRegex = /^(\d{2})\/(\d{2})\/(\d{4})$/;
      const match = value.match(dateRegex);
      
      if (match) {
        const day = parseInt(match[1], 10);
        const month = parseInt(match[2], 10) - 1; // Los meses en JS son 0-indexados
        const year = parseInt(match[3], 10);
        
        // Validar fecha
        if (this.isValidDate(day, month + 1, year)) {
          const date = new Date(year, month, day);
          
          // Verificar que la fecha sea válida
          if (date.getDate() === day && date.getMonth() === month && date.getFullYear() === year) {
            this.selectedDate.set(date);
            this.displayDate.set(new Date(date));
            this.onChange(this.selectedDate());
            this.dateChange.emit(this.selectedDate());
          } else {
            isValid = false;
            errorType = 'invalidDate';
          }
        } else {
          isValid = false;
          errorType = 'invalidDate';
        }
      } else {
        // No tiene formato de fecha correcto
        isValid = false;
        errorType = 'invalidDate';
      }
    }
    
    // Actualizar el FormControl
    if (this.formControl) {
      this.formControl.markAsTouched();
      
      if (isValid) {
        this.formControl.setValue(this.selectedDate());
        this.formControl.setErrors(null);
        this.formControl.updateValueAndValidity();
      } else {
        // NO borrar el contenido del input - solo marcar como inválido
        // NO tocar selectedDate para preservar el contenido del input
        
        if (errorType === 'required') {
          this.formControl.setValue(null);
          this.formControl.setErrors({ 'required': true });
          this.formControl.updateValueAndValidity();
        } else if (errorType === 'invalidDate') {
          // Para fechas inválidas, NO tocar el FormControl en absoluto
          // Solo marcar como touched para mostrar errores visuales
          // NO llamar setValue, setErrors, ni updateValueAndValidity
        }
      }
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.closeCalendar();
    }
  }
}
