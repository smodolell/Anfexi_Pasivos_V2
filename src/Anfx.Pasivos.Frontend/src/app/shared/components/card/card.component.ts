import { Component, ChangeDetectionStrategy, input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type CardVariant =
  | 'primary'
  | 'success'
  | 'warning'
  | 'info'
  | 'danger'
  | 'secondary'
  | 'light'
  | 'none';

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
    <div class="gc-card">
      <!-- Header personalizado (slot="header") -->
      <ng-content select="[slot=header]"></ng-content>

      <!-- Título declarativo: fondo de color según variante, texto blanco -->
      @if (title()) {
        <div class="gc-card__title-area gc-header--{{ variant() }}">
          <div class="d-flex align-items-center gap-2 flex-grow-1 min-w-0">
            @if (icon()) {
              <i [class]="icon + ' gc-card__icon flex-shrink-0'"></i>
            }
            <div class="min-w-0">
              <h5 class="gc-card__title mb-0 text-truncate">{{ title() }}</h5>
              @if (subtitle()) {
                <small class="gc-card__subtitle">{{ subtitle() }}</small>
              }
            </div>
          </div>
          <!-- Acciones opcionales en el header (botón X, badges, etc.) -->
          <ng-content select="[slot=actions]"></ng-content>
        </div>
      }

      <!-- Body -->
      <div class="card-body" [class.p-0]="noPadding()">
        @if (loading()) {
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
  styles: [
    `
      :host {
        display: block;
      }

      /* ── Card base ─────────────────────────────────── */
      .gc-card {
        border-radius: var(--border-radius-xl);
        overflow: hidden;
        background: #fff;
        border: none;
        box-shadow: var(--shadow-md);
        transition: box-shadow 0.3s ease;
      }
      .gc-card:hover {
        box-shadow: var(--shadow-lg);
      }

      /* ── Título area ────────────────────────────────── */
      .gc-card__title-area {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        padding: 12px 20px;
      }

      /* Texto blanco sobre todos los headers con gradiente */
      .gc-card__icon {
        font-size: 1.1rem;
        color: #fff;
      }
      .gc-card__title {
        font-size: 1rem;
        font-weight: 600;
        color: #fff;
      }
      .gc-card__subtitle {
        font-size: 0.8rem;
        display: block;
        color: rgba(255, 255, 255, 0.82);
      }

      /* ── Headers con gradiente — mismos valores que WrapKit / Alerta PLD ── */
      .gc-header--warning {
        background: linear-gradient(135deg, #ed8936, #dd6b20);
      }
      .gc-header--info {
        background: linear-gradient(135deg, #4299e1, #3182ce);
      }
      .gc-header--primary {
        background: linear-gradient(135deg, #015baa, #00325d);
      }
      .gc-header--success {
        background: linear-gradient(135deg, #48bb78, #38a169);
      }
      .gc-header--danger {
        background: linear-gradient(135deg, #f56565, #e53e3e);
      }
      .gc-header--secondary {
        background: linear-gradient(135deg, #718096, #4a5568);
      }

      /* ── Variantes sin gradiente (neutras) ── */
      .gc-header--light {
        background: var(--color-bg-subtle);
        border-bottom: 1px solid var(--color-border-light);
        .gc-card__title,
        .gc-card__icon {
          color: var(--color-text-primary);
        }
        .gc-card__subtitle {
          color: var(--color-text-muted);
        }
      }
      .gc-header--none {
        background: transparent;
        border-bottom: 1px solid rgba(0, 0, 0, 0.06);
        .gc-card__title,
        .gc-card__icon {
          color: var(--color-text-primary);
        }
        .gc-card__subtitle {
          color: var(--color-text-muted);
        }
      }

      /* ── Footer slot ──────────────────────────────── */
      ::ng-deep [slot='footer'] {
        display: block;
        padding: 12px 20px;
        background: #f8fafc;
        border-top: 1px solid rgba(0, 0, 0, 0.06);
      }
    `,
  ],
})
export class CardComponent {
  /** Título mostrado en el header declarativo */
  title = input<string>();
  /** Subtítulo opcional bajo el título */
  subtitle = input<string>();
  /** Clase de ícono FontAwesome, ej: 'fa-solid fa-user' */
  icon = input<string>();
  /** Color del header */
  variant = input<CardVariant>('primary');
  /** Muestra spinner en el body y oculta el contenido */
  loading = input(false);
  /** Elimina el padding del card-body (útil para tablas flush) */
  noPadding = input(false);
}
