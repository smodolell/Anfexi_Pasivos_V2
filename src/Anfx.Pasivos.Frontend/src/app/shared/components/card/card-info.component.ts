import {
  Component,
  input,
  computed,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Variantes de color para el card-header.
 * Mapea directamente a clases de Bootstrap 5 + tokens del proyecto.
 */
export type CardVariant =
  | 'primary'
  | 'secondary'
  | 'success'
  | 'danger'
  | 'warning'
  | 'info'
  | 'light'
  | 'dark'
  | 'light-blue';   // variante custom usada en Alerta

/**
 * CardComponent — card reutilizable con header coloreable.
 *
 * USO BÁSICO:
 *   <app-card title="Mi título" variant="primary">
 *     <p>Contenido libre</p>
 *   </app-card>
 *
 * CON BADGE Y FOOTER:
 *   <app-card title="Usuarios" icon="fa-solid fa-users"
 *             variant="info" [badge]="totalUsuarios()">
 *     <!-- contenido -->
 *     <div slot="footer">
 *       <button class="btn btn-primary">Guardar</button>
 *     </div>
 *   </app-card>
 *
 * SIN HEADER:
 *   <app-card [showHeader]="false">
 *     <p>Solo contenido</p>
 *   </app-card>
 *
 * ANCHO CUSTOM:
 *   <app-card title="Estrecho" [colClass]="'col-md-4'">...</app-card>
 */
@Component({
  selector: 'app-card-info',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="card card-rounded shadow-sm mb-4" [class]="cardClass()">
      @if(getIcon()){
        <i class="getIcon() me-2"></i>
      }
      <!-- ── HEADER ─────────────────────────────────────────── -->
      @if (showHeader()) {
        <div class="card-header d-flex justify-content-between align-items-center"
             [class]="headerClass()">

          <div class="d-flex align-items-center gap-2">
            @if (icon()) {
              <i [class]="icon()" aria-hidden="true"></i>
            }
            <span class="fw-semibold mb-0">{{ title() }}</span>
          </div>

          <!-- Badge + acciones del header (slot "header-actions") -->
          <div class="d-flex align-items-center gap-2">
            @if (badge() !== null && badge() !== undefined) {
              <span class="badge" [class]="badgeClass()">{{ badge() }}</span>
            }
            <ng-content select="[slot=header-actions]" />
          </div>

        </div>
        <span class="badge bg-white text-primary fs-6">{{ info() }}</span>
      }

      <!-- ── BODY ───────────────────────────────────────────── -->
      <div class="card-body" [class]="bodyClass()">
        <ng-content />
      </div>

      <!-- ── FOOTER (opcional) ──────────────────────────────── -->
      <ng-content select="[slot=footer]" />

    </div>
  `,
})
export class CardInfoComponent {

  // ── Inputs ────────────────────────────────────────────────────

  /** Texto del header */
  title       = input<string>('');

  /** Clase de Font Awesome para el ícono del header. Ej: 'fa-solid fa-users' */
  icon        = input<string>('');

  /**
   * Variante de color del header.
   * @default 'light-blue'
   */
  variant     = input<CardVariant>('light-blue');

  /** Texto del badge en el header (número o string). null/undefined = oculto */
  badge       = input<string | number | null>(null);

  /** Mostrar/ocultar el header completo */
  showHeader  = input<boolean>(true);

  info = input<string>('');
  /**
   * Clase CSS extra para el wrapper del card.
   * Útil para controlar ancho o márgenes desde el padre.
   * Ej: 'col-md-6', 'w-50', 'mx-auto'
   */
  wrapperClass = input<string>('');

  /**
   * Padding del card-body.
   * 'default' usa el padding de Bootstrap, 'none' quita el padding (útil para tablas).
   */
  bodyPadding  = input<'default' | 'none'>('default');

  // ── Computed ──────────────────────────────────────────────────

  /** Clase del wrapper externo del card */
  cardClass = computed(() => this.wrapperClass());

  /** Clase del header según variante */
  headerClass = computed((): string => {
    const v = this.variant();
    // light-blue es clase custom del proyecto (Alerta)
    if (v === 'light-blue') return 'bg-light-blue';
    // light usa texto oscuro
    if (v === 'light')      return 'bg-light text-dark';
    // el resto usa texto blanco
    return `bg-${v} text-white`;
  });

  /** Clase del badge — contraste adaptado según variante del header */
  badgeClass = computed((): string => {
    const v = this.variant();
    if (v === 'light' || v === 'light-blue') {
      return 'bg-primary text-white';
    }
    return `bg-white text-${v}`;
  });

  /** Clase del body según padding elegido */
  bodyClass = computed((): string =>
    this.bodyPadding() === 'none' ? 'p-0' : ''
  );
  getIcon():string | null {
    return this.icon();
  }
}
