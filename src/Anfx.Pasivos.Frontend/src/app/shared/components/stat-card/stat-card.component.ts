import {
  Component,
  input,
  computed,
  ChangeDetectionStrategy,
} from '@angular/core';
import { NgClass, CommonModule } from '@angular/common';

// ── Tipos públicos exportados para uso en el componente padre ────────────────

export type StatCardColor = 'primary' | 'success' | 'warning' | 'info' | 'danger';

export interface StatCardData {
  label:    string;
  value:    string;
  icon:     string;
  color:    StatCardColor;
  sublabel: string;
  trend?:   number | null;
}

/**
 * StatCardComponent — tarjeta KPI reutilizable con glow, border-left
 * de color e indicador de tendencia opcional.
 *
 * USO BÁSICO:
 *   <app-stat-card
 *     label="Capital Activo"
 *     value="$1,234,567.00"
 *     icon="fa-solid fa-arrow-trend-up"
 *     color="primary"
 *     sublabel="Cartera Activa">
 *   </app-stat-card>
 *
 * CON TENDENCIA:
 *   <app-stat-card ... [trend]="5.2"></app-stat-card>   // +5.2% verde
 *   <app-stat-card ... [trend]="-2.1"></app-stat-card>  // -2.1% rojo
 *
 * COLORES DISPONIBLES: 'primary' | 'success' | 'warning' | 'info' | 'danger'
 */
@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [NgClass, CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="sc-card" [ngClass]="colorClass()">

      <div class="sc-icon">
        <i [class]="icon()"></i>
      </div>

      <div class="sc-body">
        <span class="sc-label">{{ label() }}</span>
        <span class="sc-value">{{ value() }}</span>

        @if (sublabel()) {
          <span class="sc-sublabel">{{ sublabel() }}</span>
        }

        @if (trend() !== null) {
          <span class="sc-trend" [ngClass]="trendClass()!">
            <i [class]="trendIcon()!"></i>
            {{ trendLabel() }}
          </span>
        }
      </div>

    </div>
  `,
  styles: [`
    /* ── Host ──────────────────────────────────────────────────── */
    :host {
      display: block;
    }

    /* ── Card base ──────────────────────────────────────────────── */
    .sc-card {
      background: var(--color-white);
      border-radius: var(--border-radius-xl);
      padding: var(--space-5);
      display: flex;
      align-items: flex-start;
      gap: var(--space-4);
      border-left: 4px solid transparent;
      transition: transform 0.3s ease, box-shadow 0.3s ease;
      overflow: hidden;
    }

    .sc-card:hover {
      transform: translateY(-2px);
    }

    /* ── Variantes de color + glow ──────────────────────────────── */
    .sc--primary {
      border-left-color: var(--color-primary-medium);
      box-shadow: var(--shadow-md), 0 0 22px rgba(49, 130, 206, 0.12);
    }
    .sc--primary:hover {
      box-shadow: var(--shadow-lg), 0 0 32px rgba(49, 130, 206, 0.18);
    }

    .sc--success {
      border-left-color: var(--color-success-dark);
      box-shadow: var(--shadow-md), 0 0 22px rgba(56, 161, 105, 0.12);
    }
    .sc--success:hover {
      box-shadow: var(--shadow-lg), 0 0 32px rgba(56, 161, 105, 0.18);
    }

    .sc--warning {
      border-left-color: var(--color-warning);
      box-shadow: var(--shadow-md), 0 0 22px rgba(237, 137, 54, 0.12);
    }
    .sc--warning:hover {
      box-shadow: var(--shadow-lg), 0 0 32px rgba(237, 137, 54, 0.18);
    }

    .sc--info {
      border-left-color: var(--color-info);
      box-shadow: var(--shadow-md), 0 0 22px rgba(23, 162, 184, 0.12);
    }
    .sc--info:hover {
      box-shadow: var(--shadow-lg), 0 0 32px rgba(23, 162, 184, 0.18);
    }

    .sc--danger {
      border-left-color: var(--color-danger-bs);
      box-shadow: var(--shadow-md), 0 0 22px rgba(245, 101, 101, 0.12);
    }
    .sc--danger:hover {
      box-shadow: var(--shadow-lg), 0 0 32px rgba(245, 101, 101, 0.18);
    }

    /* ── Ícono ──────────────────────────────────────────────────── */
    .sc-icon {
      width: 50px;
      height: 50px;
      border-radius: var(--border-radius-lg);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.2rem;
      flex-shrink: 0;
    }

    .sc--primary .sc-icon { background: rgba(49, 130, 206, 0.12); color: var(--color-primary-medium); }
    .sc--success .sc-icon { background: rgba(56, 161, 105, 0.12); color: var(--color-success-dark);   }
    .sc--warning .sc-icon { background: rgba(237, 137, 54, 0.12); color: var(--color-warning);         }
    .sc--info    .sc-icon { background: rgba(23, 162, 184, 0.12); color: var(--color-info);             }
    .sc--danger  .sc-icon { background: rgba(245, 101, 101, 0.12); color: var(--color-danger-bs);      }

    /* ── Cuerpo del texto ───────────────────────────────────────── */
    .sc-body {
      display: flex;
      flex-direction: column;
      gap: 2px;
      flex: 1;
      min-width: 0;
    }

    .sc-label {
      font-size: var(--font-size-xs);
      color: var(--color-text-muted);
      font-weight: var(--font-weight-semibold);
      text-transform: uppercase;
      letter-spacing: 0.07em;
    }

    .sc-value {
      font-size: 1.05rem;
      font-weight: var(--font-weight-bold);
      color: var(--color-text-title);
      line-height: 1.3;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .sc-sublabel {
      font-size: var(--font-size-xs);
      color: var(--color-text-muted);
      margin-top: 1px;
    }

    /* ── Indicador de tendencia ─────────────────────────────────── */
    .sc-trend {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-size: var(--font-size-xs);
      font-weight: var(--font-weight-semibold);
      margin-top: var(--space-2);
      border-radius: var(--border-radius-sm);
      padding: 2px 8px;
      width: fit-content;
    }

    .sc-trend--up      { color: var(--color-success-dark); background: rgba(56, 161, 105, 0.10);   }
    .sc-trend--down    { color: var(--color-danger-bs);    background: rgba(245, 101, 101, 0.10);  }
    .sc-trend--neutral { color: var(--color-text-muted);   background: var(--color-bg-table-head); }
  `],
})
export class StatCardComponent {

  // ── Inputs requeridos ────────────────────────────────────────────
  label = input.required<string>();
  value = input.required<string>();
  icon  = input.required<string>();

  // ── Inputs opcionales ────────────────────────────────────────────
  color    = input<StatCardColor>('primary');
  sublabel = input<string>('');
  trend    = input<number | null>(null);

  // ── Computed ─────────────────────────────────────────────────────

  readonly colorClass = computed(() => `sc--${this.color()}`);

  readonly trendClass = computed((): string | null => {
    const t = this.trend();
    if (t === null) return null;
    if (t > 0)  return 'sc-trend--up';
    if (t < 0)  return 'sc-trend--down';
    return 'sc-trend--neutral';
  });

  readonly trendIcon = computed((): string | null => {
    const t = this.trend();
    if (t === null) return null;
    if (t > 0)  return 'fa-solid fa-arrow-trend-up';
    if (t < 0)  return 'fa-solid fa-arrow-trend-down';
    return 'fa-solid fa-minus';
  });

  readonly trendLabel = computed((): string | null => {
    const t = this.trend();
    if (t === null) return null;
    return `${t > 0 ? '+' : ''}${t.toFixed(1)}%`;
  });
}
