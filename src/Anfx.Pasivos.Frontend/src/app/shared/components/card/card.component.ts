import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

export type CardVariant = 'primary' | 'success' | 'warning' | 'info' | 'danger' | 'secondary' | 'light' | 'none';

/**
 * Card genérica con proyección de contenido.
 *
 * Uso básico (título e ícono declarativos):
 *   <app-card title="Nuevo Usuario" icon="fa-solid fa-user-plus" variant="info">
 *     <p>Contenido del body</p>
 *   </app-card>
 *
 * Footer personalizado:
 *   <app-card title="..." variant="warning">
 *     <form id="myForm" ...>campos...</form>
 *     <div slot="footer">
 *       <button type="submit" form="myForm">Guardar</button>
 *     </div>
 *   </app-card>
 *
 * Header completamente personalizado:
 *   <app-card>
 *     <div slot="header">...cualquier contenido...</div>
 *     <p>Body</p>
 *   </app-card>
 *
 * Acciones en el header (botón cerrar, etc.):
 *   <app-card title="..." variant="primary">
 *     <button slot="actions" ...>X</button>
 *     <p>Body</p>
 *   </app-card>
 */
@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="gc-card card">

      <!-- Header personalizado (slot="header") -->
      <ng-content select="[slot=header]"></ng-content>

      <!-- Header declarativo (cuando se usan inputs title/icon/variant) -->
      @if (title) {
        <div class="gc-card__header card-header gc-header--{{ variant }}">
          <div class="d-flex align-items-center gap-2 flex-grow-1 min-w-0">
            @if (icon) {
              <i [class]="icon + ' gc-card__icon flex-shrink-0'"></i>
            }
            <div class="min-w-0">
              <h5 class="gc-card__title mb-0 text-truncate">{{ title }}</h5>
              @if (subtitle) {
                <small class="gc-card__subtitle opacity-75">{{ subtitle }}</small>
              }
            </div>
          </div>
          <!-- Acciones opcionales en el header (botón X, badges, etc.) -->
          <ng-content select="[slot=actions]"></ng-content>
        </div>
      }

      <!-- Body -->
      <div class="card-body" [class.p-0]="noPadding">
        @if (loading) {
          <div class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Cargando...</span>
            </div>
          </div>
        }
        <ng-content></ng-content>
      </div>

      <!-- Footer opcional (slot="footer") -->
      <ng-content select="[slot=footer]"></ng-content>

    </div>
  `,
  styles: [`
    :host { display: block; }

    .gc-card {
      border-radius: 12px;
      overflow: hidden;
      box-shadow: 0 2px 10px rgba(0,0,0,0.07);
      border: 1px solid var(--pf-card-border);
    }

    .gc-card__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 12px;
      padding: 14px 20px;
      color: #fff;
    }

    .gc-card__icon { font-size: 1.15rem; }

    .gc-card__title { font-size: 1rem; font-weight: 600; }

    .gc-card__subtitle { font-size: 0.8rem; display: block; }

    /* ── Variantes de color ───────────────────────── */
    .gc-header--primary   { background: linear-gradient(135deg, var(--pf-primary), var(--pf-primary-active)); }
    .gc-header--success   { background: linear-gradient(135deg, var(--pf-success), #126832); }
    .gc-header--warning   { background: linear-gradient(135deg, var(--pf-warning), #92400e); }
    .gc-header--info      { background: linear-gradient(135deg, var(--pf-info), #025180); }
    .gc-header--danger    { background: linear-gradient(135deg, var(--pf-danger), #991b1b); }
    .gc-header--secondary { background: linear-gradient(135deg, #7f8c8d, #5d6d7e); }
    .gc-header--light     { background: var(--pf-primary-light); color: #1a1a2e !important;
                            border-bottom: 1px solid var(--pf-border); }
    .gc-header--light .gc-card__title,
    .gc-header--light .gc-card__subtitle { color: #1a1a2e; }
    .gc-header--none      { display: none; }

    /* ── Footer slot ─────────────────────────────── */
    ::ng-deep [slot=footer] {
      display: block;
      padding: 12px 20px;
      background: #f8fafc;
      border-top: 1px solid var(--pf-card-border);
    }
  `],
})
export class CardComponent {
  /** Título mostrado en el header declarativo */
  @Input() title?: string;
  /** Subtítulo opcional bajo el título */
  @Input() subtitle?: string;
  /** Clase de ícono FontAwesome, ej: 'fa-solid fa-user' */
  @Input() icon?: string;
  /** Color del header */
  @Input() variant: CardVariant = 'primary';
  /** Muestra spinner en el body y oculta el contenido */
  @Input() loading = false;
  /** Elimina el padding del card-body (útil para tablas flush) */
  @Input() noPadding = false;
}
