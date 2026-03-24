import { ChangeDetectionStrategy, Component, ElementRef, ViewChild, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-delete-modal',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './confirm-delete-modal.component.html'
})
export class ConfirmDeleteModalComponent {
  /** Etiqueta del tipo de entidad: "la actividad", "el usuario", etc. */
  itemLabel = input('el registro');
  /** Nombre/descripción específica del item a eliminar */
  itemName  = input('');

  confirmed = output<void>();
  cancelled = output<void>();

  @ViewChild('modalEl') modalEl!: ElementRef<HTMLElement>;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private modalInstance: any = null;

  show(): void {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const win = window as any;
    if (typeof window !== 'undefined' && win['bootstrap']) {
      this.modalInstance = new win['bootstrap'].Modal(this.modalEl.nativeElement);
      this.modalInstance.show();
    }
  }

  hide(): void {
    if (this.modalInstance) {
      this.modalInstance.hide();
      this.modalInstance = null;
    }
  }

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.hide();
    this.cancelled.emit();
  }
}
