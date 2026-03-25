import {
  Component, ViewChild, ElementRef, OnDestroy, ChangeDetectionStrategy,
  input,
  output,
  signal,
} from '@angular/core';
import { GenericButtonComponent } from '../generic-button/generic-button.component';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  imports: [GenericButtonComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div #modalEl class="modal fade" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content cm-content">

          <!-- Header -->
          <div class="modal-header cm-header">
            <div class="cm-header__icon">
              <i [class]="icon()"></i>
            </div>
            <h5 class="modal-title cm-header__title">{{ title() }}</h5>
            <button type="button" class="btn-close btn-close-white"
              (click)="onCancel()" aria-label="Cerrar"></button>
          </div>

          <!-- Body -->
          <div class="modal-body cm-body">
            <p class="cm-body__message">{{ message() }}</p>
            @if (itemName()) {
              <p class="cm-body__item">
                <i class="fa-solid fa-tag cm-body__tag"></i>
                <strong>{{ itemName() }}</strong>
              </p>
            }
            <p class="cm-body__warning">
              <i class="fa-solid fa-triangle-exclamation me-1"></i>
              {{ warningText() }}
            </p>
          </div>

          <!-- Footer -->
          <div class="modal-footer cm-footer">
            <app-button
              [label]="cancelLabel()"
              variant="secondary"
              (clicked)="onCancel()">
            </app-button>
            <app-button
              [icon]="confirmIcon()"
              [label]="confirmLabel()"
              [variant]="confirmVariant()"
              [loading]="confirmLoading()"
              (clicked)="onConfirm()">
            </app-button>
          </div>

        </div>
      </div>
    </div>
  `,
  styles: [`
    .cm-content {
      border: none;
      border-radius: 14px;
      overflow: hidden;
      box-shadow: 0 20px 60px rgba(0,0,0,.18);
    }

    .cm-header {
      background: linear-gradient(135deg, #e74c3c, #c0392b);
      color: #fff;
      padding: 16px 20px;
      border-bottom: none;
      gap: 12px;

      &__icon {
        width: 34px;
        height: 34px;
        border-radius: 50%;
        background: rgba(255,255,255,.2);
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        font-size: 1rem;
      }

      &__title {
        flex: 1;
        font-size: 1rem;
        font-weight: 600;
        margin: 0;
      }
    }

    .cm-body {
      padding: 24px 24px 8px;

      &__message {
        color: #374151;
        margin-bottom: 12px;
        font-size: .9rem;
      }

      &__item {
        display: flex;
        align-items: center;
        gap: 8px;
        background: #f8fafc;
        border: 1px solid #e2e8f0;
        border-radius: 8px;
        padding: 10px 14px;
        margin-bottom: 14px;
        font-size: .9rem;
        color: #1e293b;
      }

      &__tag { color: #64748b; font-size: .8rem; }

      &__warning {
        font-size: .8rem;
        color: #b45309;
        background: #fffbeb;
        border: 1px solid #fde68a;
        border-radius: 8px;
        padding: 8px 12px;
        margin-bottom: 0;
      }
    }

    .cm-footer {
      padding: 16px 20px;
      background: #f8fafc;
      border-top: 1px solid #e2e8f0;
      gap: 8px;
    }
  `],
})
export class ConfirmModalComponent implements OnDestroy {
title          = input('Confirmar eliminación');
message        = input('¿Está seguro que desea eliminar este registro?');
itemName       = input<string>();
warningText    = input('Esta acción no se puede deshacer.');
icon           = input('fa-solid fa-trash-can');
confirmLabel   = input('Eliminar');
confirmIcon    = input('fa-solid fa-trash-can');
confirmVariant = input<'danger' | 'warning' | 'primary'>('danger');
cancelLabel    = input('Cancelar');
confirmLoading = signal(false);
confirmed      = output<void>();
cancelled      = output<void>();


  @ViewChild('modalEl') modalEl!: ElementRef<HTMLElement>;

  private bsModal?: any;

  show(): void {
    this.bsModal = new (globalThis as any).bootstrap.Modal(this.modalEl.nativeElement);
    this.bsModal.show();
  }

  hide(): void {
    this.bsModal?.hide();
  }

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.hide();
    this.cancelled.emit();
  }

  ngOnDestroy(): void {
    this.bsModal?.dispose();
  }
}
