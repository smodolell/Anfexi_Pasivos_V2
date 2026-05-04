import {
  Component,
  input,
  computed,
  ChangeDetectionStrategy,
} from '@angular/core';
import { NgClass } from '@angular/common';

// ── Tipos públicos exportados ────────────────────────────────────────────────

export type AppCardVariant =
  | 'primary'
  | 'secondary'
  | 'success'
  | 'warning'
  | 'info'
  | 'dark';

/**
 * AppCardComponent — contenedor de sección reutilizable con header institucional.
 *
 * Úsalo para envolver Filtros, Tablas y Gráficos que necesitan
 * título, ícono y acciones de header. Para KPIs usa StatCardComponent.
 *
 * USO BÁSICO:
 *   <app-card title="Contratos Pasivos" icon="fa-solid fa-file-contract">
 *     <form>...</form>
 *   </app-card>
 *
 * CON ACCIONES EN HEADER:
 *   <app-card title="Resultados" icon="fa-solid fa-list" [badge]="totalCount()">
 *     <button header-actions class="btn btn-sm">Exportar</button>
 *     <table>...</table>
 *   </app-card>
 *
 * TABLA SIN PADDING:
 *   <app-card title="..." bodyPadding="none">
 *     <table class="table">...</table>
 *     <div footer class="dash-pagination">...</div>
 *   </app-card>
 *
 * VARIANTES: 'primary' | 'secondary' | 'success' | 'warning' | 'info' | 'dark'
 */
@Component({
  selector: 'app-card',
  standalone: true,
  imports: [NgClass],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="ac-card" [ngClass]="variantClass()">

      @if (showHeader()) {
        <div class="ac-header">

          <div class="ac-header-left">
            @if (icon()) {
              <i [class]="icon()" aria-hidden="true"></i>
            }
            <span class="ac-title">{{ title() }}</span>
            @if (subtitle()) {
              <span class="ac-subtitle">{{ subtitle() }}</span>
            }
          </div>

          <div class="ac-header-right">
            @if (badge() !== null && badge() !== undefined) {
              <span class="ac-badge">{{ badge() }}</span>
            }
            <ng-content select="[header-actions]" />
          </div>

        </div>
      }

      <div class="ac-body" [class.ac-body--flush]="bodyPadding() === 'none'">
        <ng-content />
      </div>

      <ng-content select="[footer]" />

    </div>
  `,
  styles: [`
    /* ── Host ──────────────────────────────────────────────────────── */
    :host {
      display: block;
    }

    /* ── Card base ──────────────────────────────────────────────────── */
    .ac-card {
      background: var(--color-white);
      border-radius: var(--border-radius-xl);
      overflow: hidden;
      transition: box-shadow 0.3s ease;
    }

    /* ── Glow por variante (hereda patrón de StatCardComponent) ─────── */
    .ac--primary   { box-shadow: var(--shadow-md), 0 0 22px rgba(49,  130, 206, 0.12); }
    .ac--secondary { box-shadow: var(--shadow-md), 0 0 22px rgba(100, 116, 139, 0.12); }
    .ac--success   { box-shadow: var(--shadow-md), 0 0 22px rgba(56,  161, 105, 0.12); }
    .ac--warning   { box-shadow: var(--shadow-md), 0 0 22px rgba(237, 137,  54, 0.12); }
    .ac--info      { box-shadow: var(--shadow-md), 0 0 22px rgba(23,  162, 184, 0.12); }
    .ac--dark      { box-shadow: var(--shadow-md), 0 0 22px rgba(30,   41,  59, 0.12); }

    /* ── Header ─────────────────────────────────────────────────────── */
    .ac-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: var(--space-3) var(--space-5);
      color: var(--color-white);
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-semibold);
      letter-spacing: 0.02em;
      gap: var(--space-3);
    }

    /* Gradiente institucional por variante */
    .ac--primary   .ac-header { background: var(--gradient-primary); }
    .ac--secondary .ac-header { background: linear-gradient(135deg, #64748b, #475569); }
    .ac--success   .ac-header { background: var(--gradient-success); }
    .ac--warning   .ac-header { background: linear-gradient(135deg, var(--color-warning, #ed8936), #c05621); }
    .ac--info      .ac-header { background: linear-gradient(135deg, #17a2b8, #0f7d8f); }
    .ac--dark      .ac-header { background: linear-gradient(135deg, #1e293b, #0f172a); }

    .ac-header-left {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      min-width: 0;
    }

    .ac-title {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-semibold);
      white-space: nowrap;
    }

    .ac-subtitle {
      font-size: var(--font-size-xs);
      opacity: 0.72;
      font-weight: var(--font-weight-normal);
      letter-spacing: 0;
      white-space: nowrap;
    }

    .ac-header-right {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      flex-shrink: 0;
    }

    /* Badge semi-transparente sobre el header (mismo estilo que dash-record-badge) */
    .ac-badge {
      background: rgba(255, 255, 255, 0.20);
      border: 1px solid rgba(255, 255, 255, 0.38);
      color: var(--color-white);
      border-radius: 20px;
      padding: 2px 12px;
      font-size: var(--font-size-xs);
      font-weight: var(--font-weight-medium);
      white-space: nowrap;
    }

    /* ── Body ───────────────────────────────────────────────────────── */
    .ac-body {
      padding: var(--space-4) var(--space-5);
    }

    .ac-body--flush {
      padding: 0;
    }

    /* ── h-100: card estira para igualar altura en una fila Bootstrap ── */
    :host(.h-100) .ac-card {
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    :host(.h-100) .ac-body {
      flex: 1;
      overflow: auto;
    }
  `],
})
export class AppCardComponent {

  // ── Inputs ────────────────────────────────────────────────────────
  title       = input<string>('');
  icon        = input<string>('');

  /** Subtítulo opcional que aparece junto al título (más pequeño, semi-opaco) */
  subtitle    = input<string>('');

  /** Variante de gradiente para el header. @default 'primary' */
  variant     = input<AppCardVariant>('primary');

  /** Número o texto que aparece como badge en el header. null = oculto */
  badge       = input<string | number | null>(null);

  /** Ocultar el header completo. @default true */
  showHeader  = input<boolean>(true);

  /** 'default' usa el padding del body; 'none' elimina el padding (para tablas) */
  bodyPadding = input<'default' | 'none'>('default');

  // ── Computed ─────────────────────────────────────────────────────
  readonly variantClass = computed(() => `ac--${this.variant()}`);
}
