import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

export type ButtonVariant = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'outline';
export type ButtonSize    = 'sm' | 'md' | 'lg';
export type ButtonType    = 'button' | 'submit' | 'reset';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './generic-button.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: [`
    :host { display: inline-flex; }

    button {
      transition: transform .15s ease, box-shadow .15s ease;

      &:not(:disabled):hover { transform: translateY(-1px); }
      &:not(:disabled):active { transform: translateY(0); box-shadow: none !important; }

      &.btn-primary:not(:disabled):hover   { box-shadow: 0 0 0 3px rgba(0,  75, 141, .20); }
      &.btn-success:not(:disabled):hover   { box-shadow: 0 0 0 3px rgba(21, 128,  61, .20); }
      &.btn-info:not(:disabled):hover      { box-shadow: 0 0 0 3px rgba(3,  105, 161, .20); }
      &.btn-danger:not(:disabled):hover    { box-shadow: 0 0 0 3px rgba(185, 28,  28, .20); }
      &.btn-warning:not(:disabled):hover   { box-shadow: 0 0 0 3px rgba(217,119,   6, .20); }
      &.btn-secondary:not(:disabled):hover { box-shadow: 0 0 0 3px rgba(127,140, 141, .20); }
    }
  `],
})
export class GenericButtonComponent {
  icon     = input<string | null>(null);
  label    = input<string | null>(null);
  variant  = input<ButtonVariant>('primary');
  size     = input<ButtonSize>('md');
  type     = input<ButtonType>('button');
  disabled = input(false);
  loading  = input(false);

  clicked = output<void>();

  btnClass = computed(() => {
    const s = this.size() === 'md' ? '' : `btn-${this.size()}`;
    return ['btn', `btn-${this.variant()}`, s].filter(Boolean).join(' ');
  });

  onClick(): void {
    if (!this.disabled() && !this.loading()) this.clicked.emit();
  }
}
