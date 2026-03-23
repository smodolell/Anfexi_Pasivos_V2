import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

export type ButtonVariant = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'outline';
export type ButtonSize    = 'sm' | 'md' | 'lg';
export type ButtonType    = 'button' | 'submit' | 'reset';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './generic-button.component.html',
  styleUrl: './generic-button.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GenericButtonComponent {
  icon     = input('');
  label    = input('');
  variant  = input<ButtonVariant>('primary');
  size     = input<ButtonSize>('md');
  type     = input<ButtonType>('button');
  disabled = input(false);
  loading  = input(false);

  clicked = output<void>();

  btnClass = computed(() => `gb-btn gb-btn--${this.variant()}`);

  onClick(): void {
    if (!this.disabled() && !this.loading()) this.clicked.emit();
  }
}
