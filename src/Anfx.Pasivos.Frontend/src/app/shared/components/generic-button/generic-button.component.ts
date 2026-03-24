import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

export type ButtonVariant = 'primary' | 'secondary' | 'success' | 'danger' | 'info' | 'warning' | 'outline';

@Component({
  selector: 'app-button',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './generic-button.component.html',
  styleUrl: './generic-button.component.scss',
})
export class GenericButtonComponent {
  label   = input('');
  icon    = input('');
  variant = input<ButtonVariant>('primary');
  loading = input(false);
  disabled = input(false);
  type    = input<'button' | 'submit'>('button');

  clicked = output<void>();

  btnClass = computed(() => `gb-btn gb-btn--${this.variant()}`);

  onClick(): void {
    if (!this.disabled() && !this.loading()) {
      this.clicked.emit();
    }
  }
}
