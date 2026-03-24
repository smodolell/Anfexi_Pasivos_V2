import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

export type ActionVariant = 'edit' | 'delete' | 'config' | 'info' | 'warning';

@Component({
  selector: 'app-action-button',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button
      type="button"
      [class]="computedClass"
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
  variant  = input<ActionVariant | undefined>(undefined);
  btnClass = input<string | undefined>(undefined);
  title    = input('');
  disabled = input(false);

  clicked = output<void>();

  get computedClass(): string {
    if (this.btnClass()) return 'action-btn ' + this.btnClass();
    return 'action-btn ' + (this.variant() ?? 'edit') + '-btn';
  }

  onClick(): void {
    if (!this.disabled()) this.clicked.emit();
  }
}
