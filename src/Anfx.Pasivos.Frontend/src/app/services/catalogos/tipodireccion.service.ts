import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CATALOGO_URL } from '../../api.config';
import { PagedResultDto, PageQueryDto } from '../../../types/paged-result.dto';
import { ApiResultDto } from '../../../types/apiresult.dto';
import { TipoDireccionDto, TipoDireccionPageQueryDto, CreateTipoDireccionDto, UpdateTipoDireccionDto } from '../../../types/catalogos/tipodireccion.dto';

@Injectable({ providedIn: 'root' })
export class TipoDireccionService {
    private readonly resource = 'tiposdirecciones';

    constructor(
        private http: HttpClient,
        @Inject(API_CATALOGO_URL) private readonly baseUrl: string
    ) {}

    getAll(params?: TipoDireccionPageQueryDto): Observable<ApiResultDto<PagedResultDto<TipoDireccionDto>>> {
        let httpParams = new HttpParams();
        if (params) {
            if (params.q)                 httpParams = httpParams.set('q', params.q);
            if (params.page != null)      httpParams = httpParams.set('page', String(params.page));
            if (params.size != null)      httpParams = httpParams.set('size', String(params.size));
        }
        return this.http.get<ApiResultDto<PagedResultDto<TipoDireccionDto>>>(`${this.baseUrl}/${this.resource}`, { params: httpParams });
    }

    getById(id: number): Observable<ApiResultDto<TipoDireccionDto>> {
        return this.http.get<ApiResultDto<TipoDireccionDto>>(`${this.baseUrl}/${this.resource}/${id}`);
    }

    create(dto: CreateTipoDireccionDto): Observable<ApiResultDto<TipoDireccionDto>> {
        return this.http.post<ApiResultDto<TipoDireccionDto>>(`${this.baseUrl}/${this.resource}`, dto);
    }

    update(id: number, dto: UpdateTipoDireccionDto): Observable<ApiResultDto<TipoDireccionDto>> {
        return this.http.put<ApiResultDto<TipoDireccionDto>>(`${this.baseUrl}/${this.resource}/${id}`, dto);
    }

    delete(id: number): Observable<ApiResultDto<void>> {
        return this.http.delete<ApiResultDto<void>>(`${this.baseUrl}/${this.resource}/${id}`);
    }

    exportar(q?: string): Observable<Blob> {
      let httpParams = new HttpParams();
      if (q) {
          httpParams = httpParams.set('q', q);
      }
      return this.http.get(`${this.baseUrl}/${this.resource}/exportar`, {
          params: httpParams,
          responseType: 'blob'
      });
    }
}
