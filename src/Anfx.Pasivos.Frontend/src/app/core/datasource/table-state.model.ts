export type LoadingType = 'init' | 'search' | 'paginate' | 'sort';

export interface TableState<T> {
  items: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  loading: boolean;
  loadingType: LoadingType | null;
}
