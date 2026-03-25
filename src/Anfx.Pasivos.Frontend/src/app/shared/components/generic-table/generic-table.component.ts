import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
  output,
} from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe, PercentPipe } from '@angular/common';
import {
  SortDirection,
  TableAction,
  TableActionEvent,
  TableColumn,
  TableSortEvent,
} from './table-column.model';
import { SearchInputComponent } from '../search-input/search-input.component';
import { GenericButtonComponent } from '../generic-button/generic-button.component';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
  selector: 'app-generic-table',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, DatePipe, CurrencyPipe, PercentPipe, SearchInputComponent, GenericButtonComponent, ActionButtonComponent],
  templateUrl: './generic-table.component.html',
  styleUrl: './generic-table.component.scss',
})
export class GenericTableComponent {
  // ── Datos ────────────────────────────────────────────────────
  columns = input<TableColumn[]>([]);
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  items   = input<any[]>([]);
  loading = input(false);
  actions = input<TableAction[]>([]);

  // ── Paginación ───────────────────────────────────────────────
  totalCount          = input(0);
  currentPage         = input(1);
  totalPages          = input(1);
  pageSize            = input(10);
  showPageSizeSelector = input(false);

  // ── Búsqueda ─────────────────────────────────────────────────
  searchable        = input(false);
  searchPlaceholder = input('Buscar...');
  searchValue       = input('');

  // ── Empty state ──────────────────────────────────────────────
  emptyMessage = input('No hay registros disponibles');
  emptyIcon    = input('fas fa-box-open');

  // ── Ordenamiento ─────────────────────────────────────────────
  sortColumn    = input<string | null>(null);
  sortDirection = input<SortDirection>('asc');

  // ── Toolbar ──────────────────────────────────────────────────
  showNew      = input(false);
  showExport   = input(false);
  exportLoading = input(false);

  // ── Skeleton ─────────────────────────────────────────────────
  skeletonRowCount = input(8);

  // ── Computed ─────────────────────────────────────────────────
  skeletonRows = computed(() =>
    Array.from({ length: this.skeletonRowCount() }, (_, i) => i)
  );

  visibleColumns = computed(() =>
    this.columns().filter(c => c.visible !== false)
  );

  startRecord = computed(() =>
    this.totalCount() === 0 ? 0 : (this.currentPage() - 1) * this.pageSize() + 1
  );

  endRecord = computed(() =>
    Math.min(this.currentPage() * this.pageSize(), this.totalCount())
  );

  hasActiveSearch = computed(() => !!this.searchValue()?.trim());

  readonly pageSizeOptions = [10, 25, 50, 100];

  // ── Eventos ──────────────────────────────────────────────────
  actionCalled    = output<TableActionEvent>();
  pageNext        = output<void>();
  pagePrev        = output<void>();
  searchChanged   = output<string>();
  clearSearch     = output<void>();
  sortChanged     = output<TableSortEvent>();
  newClicked      = output<void>();
  exportClicked   = output<void>();
  pageSizeChanged = output<number>();

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onAction(actionId: string, row: any): void {
    this.actionCalled.emit({ action: actionId, row });
  }

  onSearch(event: string | Event): void {
    let value = '';

    if (typeof event === 'string') {
      value = event;
    } else {
      value = (event.target as HTMLInputElement).value;
    }
    this.searchChanged.emit(value);

  }

  onClearSearch(): void {
    this.clearSearch.emit();
    this.searchChanged.emit('');
  }

  onSort(col: TableColumn): void {
    if (!col.sortable) return;
    const direction: SortDirection =
      this.sortColumn() === col.key && this.sortDirection() === 'asc' ? 'desc' : 'asc';
    this.sortChanged.emit({ column: col.key, direction });
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSizeChanged.emit(value);
  }

  /** Resuelve dot-notation: 'address.city' => item.address.city */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  resolveValue(item: any, key: string): any {
    // eslint-disable-next-line @typescript-eslint/no-unsafe-return, @typescript-eslint/no-unsafe-member-access
    return key.split('.').reduce((obj, k) => obj?.[k] ?? '', item);
  }

  /** Clases Bootstrap para ocultar columna por breakpoint */
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
