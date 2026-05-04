import { inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { retry, timeout } from 'rxjs/operators';
import { ApiResultDto } from '../../types/apiresult.dto';
import { PagedResultDto, PageQueryDto } from '../../types/paged-result.dto';
import { API_BASE_URL } from '../api.config';

const HTTP_TIMEOUT_MS = 30_000;
const GET_RETRY_COUNT = 2;

export abstract class BaseCatalogService<
  TList,
  TDetail  = TList,
  TCreate  = TList,
  TUpdate  = Partial<TCreate>
> {
  protected readonly http    = inject(HttpClient);
  protected readonly baseUrl = inject(API_BASE_URL);

  protected abstract readonly resource: string;

  protected get endpoint(): string {
    return `${this.baseUrl}/${this.resource}`;
  }

  // ── CRUD estándar ────────────────────────────────────────────

  getAll(query?: PageQueryDto): Observable<ApiResultDto<PagedResultDto<TList>>> {
    return this.http
      .get<ApiResultDto<PagedResultDto<TList>>>(this.endpoint, { params: this.buildParams(query) })
      .pipe(timeout(HTTP_TIMEOUT_MS), retry(GET_RETRY_COUNT));
  }

  getById(id: number): Observable<ApiResultDto<TDetail>> {
    return this.http
      .get<ApiResultDto<TDetail>>(`${this.endpoint}/${id}`)
      .pipe(timeout(HTTP_TIMEOUT_MS), retry(GET_RETRY_COUNT));
  }

  create(dto: TCreate): Observable<ApiResultDto<TList>> {
    return this.http
      .post<ApiResultDto<TList>>(this.endpoint, dto)
      .pipe(timeout(HTTP_TIMEOUT_MS));
  }

  update(id: number, dto: TUpdate): Observable<ApiResultDto<TList>> {
    return this.http
      .put<ApiResultDto<TList>>(`${this.endpoint}/${id}`, dto)
      .pipe(timeout(HTTP_TIMEOUT_MS));
  }

  delete(id: number): Observable<ApiResultDto<void>> {
    return this.http
      .delete<ApiResultDto<void>>(`${this.endpoint}/${id}`)
      .pipe(timeout(HTTP_TIMEOUT_MS));
  }

  // ── Utilidades ───────────────────────────────────────────────

  /**
   * Subclases pueden sobreescribir para agregar params adicionales:
   *
   *   protected override buildParams(query?: MyQuery): HttpParams {
   *     return super.buildParams(query).set('estado', query?.estado ?? '');
   *   }
   */
  protected buildParams(query?: PageQueryDto): HttpParams {
    let params = new HttpParams();
    if (!query) return params;
    if (query.q)       params = params.set('q',       query.q);
    if (query.page)    params = params.set('page',    String(query.page));
    if (query.size)    params = params.set('size',    String(query.size));
    if (query.sortBy)  params = params.set('sortBy',  query.sortBy);
    if (query.sortDir) params = params.set('sortDir', query.sortDir);
    return params;
  }
}
