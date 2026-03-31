export interface TableQuery {
  q?: string;
  page: number;
  size: number;
  sortBy?: string;
  sortDir?: 'asc' | 'desc';
  activo?: boolean
}
