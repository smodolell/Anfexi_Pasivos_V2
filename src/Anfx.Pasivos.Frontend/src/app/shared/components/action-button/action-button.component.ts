import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

export type ActionVariant = 'edit' | 'delete' | 'config' | 'info' | 'warning';

@Component({
  selector: 'app-action-button',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button
      type="button"
      [class]="'action-btn ' + variant() + '-btn'"
      [title]="title()"
      [disabled]="disabled()"
      (click)="onClick()"
    >
      <i [class]="icon()" aria-hidden="true"></i>
    </button>
  `,
})
export class ActionButtonComponent {
  icon     = input('');
  variant  = input<ActionVariant>('edit');
  title    = input('');
  disabled = input(false);

  clicked = output<void>();

  onClick(): void {
    if (!this.disabled()) this.clicked.emit();
  }
}
