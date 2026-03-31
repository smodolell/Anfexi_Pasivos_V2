export interface PagedResponse<T> {
  results?: T[] | null;
  currentPage?: number;
  pageSize?: number;
  totalCount?: number;
  totalPages?: number;
}
