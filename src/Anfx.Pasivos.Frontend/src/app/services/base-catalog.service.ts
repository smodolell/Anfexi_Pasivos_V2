import { inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResultDto } from '../../types/apiresult.dto';
import { PagedResultDto, PageQueryDto } from '../../types/paged-result.dto';

/**
 * Servicio base genérico para catálogos CRUD con paginación server-side.
 *
 * Uso:
 *
 *   @Injectable({ providedIn: 'root' })
 *   export class ColoniaService extends BaseCatalogService<ColoniaDto, ColoniaDto, CreateColoniaDto, UpdateColoniaDto> {
 *     protected readonly baseUrl  = inject(API_CATALOGO_URL);
 *     protected readonly resource = 'colonias';
 *   }
 *
 * Parámetros de tipo:
 *   TList   – DTO que devuelve el listado paginado
 *   TDetail – DTO que devuelve getById          (default = TList)
 *   TCreate – DTO que recibe create             (default = TList)
 *   TUpdate – DTO que recibe update             (default = Partial<TCreate>)
 */
export abstract class BaseCatalogService<
  TList,
  TDetail  = TList,
  TCreate  = TList,
  TUpdate  = Partial<TCreate>
> {
  protected readonly http = inject(HttpClient);

  /** URL base del API, ej: inject(API_CATALOGO_URL) */
  protected abstract readonly baseUrl: string;

  /** Segmento del recurso, ej: 'colonias' */
  protected abstract readonly resource: string;

  protected get endpoint(): string {
    return `${this.baseUrl}/${this.resource}`;
  }

  // ── CRUD estándar ────────────────────────────────────────────

  getAll(query?: PageQueryDto): Observable<ApiResultDto<PagedResultDto<TList>>> {
    return this.http.get<ApiResultDto<PagedResultDto<TList>>>(this.endpoint, {
      params: this.buildParams(query),
    });
  }

  getById(id: number): Observable<ApiResultDto<TDetail>> {
    return this.http.get<ApiResultDto<TDetail>>(`${this.endpoint}/${id}`);
  }

  create(dto: TCreate): Observable<ApiResultDto<TList>> {
    return this.http.post<ApiResultDto<TList>>(this.endpoint, dto);
  }

  update(id: number, dto: TUpdate): Observable<ApiResultDto<TList>> {
    return this.http.put<ApiResultDto<TList>>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResultDto<void>> {
    return this.http.delete<ApiResultDto<void>>(`${this.endpoint}/${id}`);
  }

  // ── Utilidades ───────────────────────────────────────────────

  /**
   * Convierte un PageQueryDto en HttpParams.
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
