export interface TableDataSourceConfig {
  cacheTTL?: number; // ms
  debounceTime?: number;
  retryCount?: number;
  storageKey?: string;
}
