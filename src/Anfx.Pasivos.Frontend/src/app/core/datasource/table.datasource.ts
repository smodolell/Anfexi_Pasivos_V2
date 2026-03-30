import { signal, computed } from '@angular/core';
import { DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, switchMap, tap, catchError, of, Observable } from 'rxjs';
import { TableQuery } from '../models/table-query.model';
import { PagedResponse } from '../models/paged-response.model';
import { ApiResponse } from '../models/api-response.model';
import { TableDataSourceConfig } from './table.datasource.config';

type LoadingType = 'init' | 'search' | 'paginate' | 'sort' | null;

export class TableDataSource<T> {
  private readonly _items = signal<T[]>([]);
  private readonly _totalCount = signal(0);
  private readonly _pageSize = signal(10);
  private readonly _currentPage = signal(1);
  private readonly _loading = signal(false);
  private readonly _loadingType = signal<LoadingType>(null);
  private readonly _error = signal<string | null>(null);
  private readonly _query = signal<TableQuery>({ page: 1, size: 10, q: '' });

  private readonly trigger$ = new Subject<{ query: TableQuery; type: NonNullable<LoadingType> }>();
  private readonly cache = new Map<string, { data: PagedResponse<T>; ts: number }>();

  private readonly debounceMs: number;
  private readonly cacheTTL: number;

  readonly items = computed(() => this._items());
  readonly totalCount = computed(() => this._totalCount());
  readonly currentPage = computed(() => this._currentPage());
  readonly pageSize = computed(() => this._pageSize());
  readonly loading = computed(() => this._loading());
  readonly loadingType = computed(() => this._loadingType());
  readonly error = computed(() => this._error());
  readonly totalPages = computed(() => {
    const t = this._totalCount(),
      s = this._pageSize();
    return t > 0 ? Math.ceil(t / s) : 0;
  });
  readonly searchValue   = computed(() => this._query().q ?? '');
  readonly sortColumn    = computed(() => this._query().sortBy ?? null);
  readonly sortDirection = computed(() => this._query().sortDir ?? 'asc');
  readonly activo        = computed(() => this._query().activo);
  constructor(
    private readonly fetchFn: (q: TableQuery) => Observable<ApiResponse<PagedResponse<T>>>,
    destroyRef: DestroyRef,
    config: TableDataSourceConfig = {},
  ) {
    this.debounceMs = config.debounceTime ?? 300;
    this.cacheTTL = config.cacheTTL ?? 120_000;

    this.trigger$
      .pipe(
        debounceTime(this.debounceMs),
        tap(({ type }) => {
          this._loading.set(true);
          this._loadingType.set(type);
          this._error.set(null);
        }),
        switchMap(({ query }) => {
          const key = JSON.stringify(query);
          const cached = this.cache.get(key);
          if (cached && Date.now() - cached.ts < this.cacheTTL) {
            this._setData(cached.data);
            return of(null);
          }
          return this.fetchFn(query).pipe(
            tap((res) => {
              if (res?.success && res.data) {
                this.cache.set(key, { data: res.data, ts: Date.now() });
                this._setData(res.data);
              } else {
                this._reset();
                this._error.set(res?.errors?.[0] ?? res?.message ?? 'Error al cargar datos');
              }
            }),
            catchError(() => {
              this._reset();
              this._error.set('Error de conexión');
              return of(null);
            }),
          );
        }),
        tap(() => {
          this._loading.set(false);
          this._loadingType.set(null);
        }),
        takeUntilDestroyed(destroyRef),
      )
      .subscribe();
  }

  load(): void {
    this._dispatch('init');
  }
  search(value: string): void {
    this._patchQuery({ q: value, page: 1 });
    this._dispatch('search');
  }
  sort(column: string, dir: 'asc' | 'desc'): void {
    this._patchQuery({ sortBy: column, sortDir: dir, page: 1 });
    this._dispatch('sort');
  }
  nextPage(): void {
    if (this._currentPage() < this.totalPages()) {
      this._patchQuery({ page: this._query().page + 1 });
      this._dispatch('paginate');
    }
  }
  prevPage(): void {
    if (this._query().page > 1) {
      this._patchQuery({ page: this._query().page - 1 });
      this._dispatch('paginate');
    }
  }
  setPageSize(size: number): void {
    this._patchQuery({ size, page: 1 });
    this._dispatch('paginate');
  }
  reload(): void {
    this.cache.clear();
    this._dispatch('init');
  }
  setFilter(patch: Pick<TableQuery, 'activo'>): void {
    this._patchQuery({ ...patch, page: 1 });
    this.cache.clear();
    this._dispatch('search');
  }
  private _patchQuery(patch: Partial<TableQuery>): void {
    this._query.set({ ...this._query(), ...patch });
  }
  private _dispatch(type: NonNullable<LoadingType>): void {
    this.trigger$.next({ query: this._query(), type });
  }
  private _setData(data: PagedResponse<T>): void {
    this._items.set(data.results ?? []);
    this._currentPage.set(data.currentPage ?? 1);
    this._pageSize.set(data.pageSize ?? 10);
    this._totalCount.set(data.totalCount ?? 0);
  }
  private _reset(): void {
    this._items.set([]);
    this._totalCount.set(0);
  }
}
