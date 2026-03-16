import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe, PercentPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableColumn, TableAction, TableActionEvent, TableSortEvent, SortDirection } from './table-column.model';
import { SearchInputComponent } from '../search-input/search-input.component';
import { GenericButtonComponent } from '../generic-button/generic-button.component';

@Component({
  selector: 'app-generic-table',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, CurrencyPipe, PercentPipe, SearchInputComponent, GenericButtonComponent],
  templateUrl: './generic-table.component.html',
  styleUrl: './generic-table.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GenericTableComponent {
  /** Definición de columnas */
  @Input() columns: TableColumn[] = [];
  /** Datos a mostrar */
  @Input() items: any[] = [];
  /** Estado de carga */
  @Input() loading = false;
  /** Botones de acción por fila */
  @Input() actions: TableAction[] = [];

  // ── Paginación ──────────────────────────────────────────────
  @Input() totalCount = 0;
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Input() pageSize = 10;

  /** Muestra selector de filas por página (10 / 25 / 50 / 100) */
  @Input() showPageSizeSelector = false;

  // ── Búsqueda ────────────────────────────────────────────────
  @Input() searchable = true;
  @Input() searchPlaceholder = 'Buscar...';
  @Input() searchValue = '';

  // ── Empty state ──────────────────────────────────────────────
  /** Mensaje cuando la tabla no tiene datos (sin búsqueda activa) */
  @Input() emptyMessage = 'No hay registros disponibles';
  /** Ícono FontAwesome para el empty state (sin búsqueda activa) */
  @Input() emptyIcon = 'fa-solid fa-box-open';

  // ── Ordenamiento server-side ─────────────────────────────────
  @Input() sortColumn: string | null = null;
  @Input() sortDirection: SortDirection = 'asc';

  // ── Acciones de toolbar ──────────────────────────────────────
  @Input() showNew = true;
  @Input() showExport = false;
  @Input() exportLoading = false;

  // ── Skeleton ─────────────────────────────────────────────────
  @Input() skeletonRowCount = 10;

  get skeletonRows(): number[] {
    return Array.from({ length: this.skeletonRowCount }, (_, i) => i);
  }

  // ── Eventos ──────────────────────────────────────────────────
  @Output() actionCalled    = new EventEmitter<TableActionEvent>();
  @Output() pageNext        = new EventEmitter<void>();
  @Output() pagePrev        = new EventEmitter<void>();
  @Output() searchChanged   = new EventEmitter<string>();
  @Output() clearSearch     = new EventEmitter<void>();
  @Output() sortChanged     = new EventEmitter<TableSortEvent>();
  @Output() newClicked      = new EventEmitter<void>();
  @Output() exportClicked   = new EventEmitter<void>();
  @Output() pageSizeChanged = new EventEmitter<number>();

  readonly pageSizeOptions = [10, 25, 50, 100];

  get visibleColumns(): TableColumn[] {
    return this.columns.filter(c => c.visible !== false);
  }

  get startRecord(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get endRecord(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  get hasActiveSearch(): boolean {
    return !!this.searchValue?.trim();
  }

  onAction(actionId: string, row: any) {
    this.actionCalled.emit({ action: actionId, row });
  }

  onSearch(value: string) {
    this.searchChanged.emit(value);
  }

  onClearSearch() {
    this.clearSearch.emit();
    this.searchChanged.emit('');
  }

  onSort(col: TableColumn) {
    if (!col.sortable) return;
    const direction: SortDirection =
      this.sortColumn === col.key && this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.sortChanged.emit({ column: col.key, direction });
  }

  onPageSizeChange(event: Event) {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSizeChanged.emit(value);
  }

  /**
   * Resuelve el valor de una propiedad con soporte a dot-notation.
   * Ej: key='address.city' => item.address.city
   */
  resolveValue(item: any, key: string): any {
    return key.split('.').reduce((obj, k) => obj?.[k] ?? '', item);
  }

  /**
   * Devuelve las clases Bootstrap para ocultar la columna
   * por debajo del breakpoint indicado en col.hideBelow.
   */
  colVisibilityClass(col: TableColumn): string {
    const map: Record<string, string> = {
      sm: 'd-none d-sm-table-cell',
      md: 'd-none d-md-table-cell',
      lg: 'd-none d-lg-table-cell',
      xl: 'd-none d-xl-table-cell',
    };
    return col.hideBelow ? map[col.hideBelow] : '';
  }
}
