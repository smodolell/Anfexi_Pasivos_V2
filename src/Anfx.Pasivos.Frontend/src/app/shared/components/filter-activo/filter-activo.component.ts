import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-filter-activo',
  standalone: true,
  template: `
    <button
      type="button"
      class="fav-pill"
      [class.fav-pill--on]="value()"
      (click)="toggle()"
      [attr.aria-pressed]="value()"
      [title]="value() ? 'Mostrando solo activos' : 'Mostrando todos'"
    >
      <i class="fav-pill__icon" [class]="value() ? 'fa-solid fa-circle-check' : 'fa-regular fa-circle'"></i>
      <span class="fav-pill__label">{{ label() }}</span>
    </button>
  `,
  styles: [`
    :host { display: inline-flex; align-items: center; }

    .fav-pill {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      height: 40px;
      padding: 0 14px;
      border: 1.5px solid #cbd5e1;
      border-radius: 10px;
      background: #fff;
      cursor: pointer;
      white-space: nowrap;
      font-size: .8rem;
      font-weight: 500;
      color: #64748b;
      transition: border-color .2s, background .2s, color .2s, box-shadow .2s;

      &:hover {
        border-color: #93c5fd;
        background: #f8faff;
      }

      &:focus-visible {
        outline: 2px solid #1d6cf5;
        outline-offset: 2px;
      }

      &--on {
        border-color: #1d6cf5;
        background: #eff6ff;
        color: #1d4ed8;
        box-shadow: 0 0 0 3px rgba(29,108,245,.12);
      }
    }

    .fav-pill__icon {
      font-size: .85rem;
      flex-shrink: 0;
      color: #94a3b8;
      transition: color .2s;

      .fav-pill--on & { color: #1d6cf5; }
    }
  `],
})
export class FilterActivoComponent {
  value   = input<boolean | undefined>(true);
  label   = input('Solo activos');
  inputId = input('filter-activo');

  valueChange = output<boolean>();
  changed     = output<boolean>();

  toggle() {
    const next = !this.value();
    this.valueChange.emit(next);
    this.changed.emit(next);
  }
}
