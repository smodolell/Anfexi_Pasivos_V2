import {
  Component, Input, Output, EventEmitter,
  ViewChild, ElementRef, ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'app-search-input',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="si-wrapper" [class.si-focused]="focused">
      <i class="fa-solid fa-magnifying-glass si-icon"></i>
      <input
        #inputRef
        type="text"
        class="si-input"
        [placeholder]="placeholder"
        [value]="value"
        (focus)="focused = true"
        (blur)="focused = false"
        (keyup.enter)="onSearch()"
        (keyup.escape)="onClear()"
      />
      @if (inputRef.value) {
        <button class="si-clear" type="button" (click)="onClear()" title="Limpiar">
          <i class="fa-solid fa-xmark"></i>
        </button>
      }
      <button class="si-btn" type="button" (click)="onSearch()">
        Buscar
      </button>
    </div>
  `,
  styles: [`
    :host { display: block; }

    .si-wrapper {
      display: flex;
      align-items: center;
      height: 40px;
      border: 1.5px solid #cbd5e1;
      border-radius: 10px;
      background: #fff;
      padding: 0 0 0 12px;
      gap: 6px;
      transition:
        border-color 0.2s ease,
        box-shadow  0.2s ease,
        background  0.2s ease;
    }

    /* Efecto al enfocar */
    .si-wrapper.si-focused {
      border-color: #1d6cf5;
      box-shadow: 0 0 0 3px rgba(29, 108, 245, 0.15);
      background: #f8fbff;
    }

    .si-icon {
      color: #94a3b8;
      font-size: 0.85rem;
      flex-shrink: 0;
      transition: color 0.2s ease;
    }
    .si-wrapper.si-focused .si-icon {
      color: #1d6cf5;
    }

    .si-input {
      flex: 1;
      border: none;
      outline: none;
      background: transparent;
      font-size: 0.875rem;
      color: #1e293b;
      min-width: 0;
    }
    .si-input::placeholder {
      color: #94a3b8;
    }

    .si-clear {
      background: none;
      border: none;
      padding: 0 4px;
      color: #94a3b8;
      cursor: pointer;
      font-size: 0.8rem;
      line-height: 1;
      flex-shrink: 0;
      transition: color 0.15s ease;
    }
    .si-clear:hover { color: #64748b; }

    .si-btn {
      height: 100%;
      padding: 0 16px;
      border: none;
      border-left: 1.5px solid #cbd5e1;
      border-radius: 0 8px 8px 0;
      background: #1d6cf5;
      color: #fff;
      font-size: 0.8rem;
      font-weight: 600;
      cursor: pointer;
      flex-shrink: 0;
      letter-spacing: 0.02em;
      transition:
        background 0.2s ease,
        border-color 0.2s ease;
    }
    .si-btn:hover  { background: #1558d6; }
    .si-btn:active { background: #1044b8; }

    .si-wrapper.si-focused .si-btn {
      border-left-color: #1d6cf5;
    }
  `],
})
export class SearchInputComponent {
  @Input() placeholder = 'Buscar...';
  @Input() value       = '';

  @Output() search = new EventEmitter<string>();
  @Output() cleared = new EventEmitter<void>();

  @ViewChild('inputRef') inputRef!: ElementRef<HTMLInputElement>;

  focused = false;

  onSearch() {
    this.search.emit(this.inputRef.nativeElement.value.trim());
  }

  onClear() {
    this.inputRef.nativeElement.value = '';
    this.cleared.emit();
    this.search.emit('');
  }
}
