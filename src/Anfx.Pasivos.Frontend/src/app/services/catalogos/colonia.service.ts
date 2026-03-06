import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CATALOGO_URL } from '../../api.config';
import { PagedResultDto, PageQueryDto } from '../../../types/paged-result.dto';
import { ApiResultDto } from '../../../types/apiresult.dto';
import { ColoniaDto, ColoniaPageQueryDto, CreateColoniaDto, UpdateColoniaDto, ColoniaComponentDto } from '../../../types/catalogos/colonia.dto';
import { SelectItemDto } from '../../../types/selectitem.dto';

@Injectable({ providedIn: 'root' })
export class ColoniaService {
    private readonly resource = 'colonias';

    constructor(
        private http: HttpClient,
        @Inject(API_CATALOGO_URL) private readonly baseUrl: string
    ) {}

    getAll(params?: ColoniaPageQueryDto): Observable<ApiResultDto<PagedResultDto<ColoniaDto>>> {
        let httpParams = new HttpParams();
        if (params) {
            if (params.q)                 httpParams = httpParams.set('q', params.q);
            if (params.page != null)      httpParams = httpParams.set('page', String(params.page));
            if (params.size != null)      httpParams = httpParams.set('size', String(params.size));
        }
        return this.http.get<ApiResultDto<PagedResultDto<ColoniaDto>>>(`${this.baseUrl}/${this.resource}`, { params: httpParams });
    }

    getById(id: number): Observable<ApiResultDto<ColoniaDto>> {
        return this.http.get<ApiResultDto<ColoniaDto>>(`${this.baseUrl}/${this.resource}/${id}`);
    }

    create(dto: CreateColoniaDto): Observable<ApiResultDto<ColoniaDto>> {
        return this.http.post<ApiResultDto<ColoniaDto>>(`${this.baseUrl}/${this.resource}`, dto);
    }

    update(id: number, dto: UpdateColoniaDto): Observable<ApiResultDto<ColoniaDto>> {
        return this.http.put<ApiResultDto<ColoniaDto>>(`${this.baseUrl}/${this.resource}/${id}`, dto);
    }

    delete(id: number): Observable<ApiResultDto<void>> {
        return this.http.delete<ApiResultDto<void>>(`${this.baseUrl}/${this.resource}/${id}`);
    }

    getCodigosPostales(codigoPostal: string): Observable<ApiResultDto<SelectItemDto[]>> {
        let httpParams = new HttpParams();
        httpParams = httpParams.set('codigoPostal', codigoPostal);
        return this.http.get<ApiResultDto<SelectItemDto[]>>(`${this.baseUrl}/${this.resource}/get-codigospostales`, { params: httpParams });
    }

    getColoniasByCodigoPostal(codigoPostal: string): Observable<ApiResultDto<ColoniaComponentDto>> {
        let httpParams = new HttpParams();
        httpParams = httpParams.set('codigoPostal', codigoPostal);
        return this.http.get<ApiResultDto<ColoniaComponentDto>>(`${this.baseUrl}/${this.resource}/${this.resource}/get-cols-by-cp`, { params: httpParams });
    }

    getColoniasById(id: number): Observable<ApiResultDto<ColoniaComponentDto>> {
        return this.http.get<ApiResultDto<ColoniaComponentDto>>(`${this.baseUrl}/${this.resource}/${this.resource}/get-cols-by-id/${id}`);
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
