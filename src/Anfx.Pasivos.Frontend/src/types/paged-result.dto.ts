export interface PagedResultDto<T> {
    results: T[];
    currentPage: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  }

export interface PageQueryDto {
    q?: string;
    page?: number;
    size?: number;
}

