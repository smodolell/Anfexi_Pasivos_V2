import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { inject } from '@angular/core';
import { API_CATALOGO_URL } from '../../api.config';
import { ApiResultDto } from '../../../types/apiresult.dto';
import { ColoniaDto, ColoniaPageQueryDto, CreateColoniaDto, UpdateColoniaDto, ColoniaComponentDto } from '../../../types/catalogos/colonia.dto';
import { SelectItemDto } from '../../../types/selectitem.dto';
import { BaseCatalogService } from '../base-catalog.service';

@Injectable({ providedIn: 'root' })
export class ColoniaService extends BaseCatalogService<ColoniaDto, ColoniaDto, CreateColoniaDto, UpdateColoniaDto> {
    protected readonly baseUrl  = inject(API_CATALOGO_URL);
    protected readonly resource = 'colonias';

    // ── Métodos específicos de colonias ──────────────────────────

    getCodigosPostales(codigoPostal: string): Observable<ApiResultDto<SelectItemDto[]>> {
        const params = new HttpParams().set('codigoPostal', codigoPostal);
        return this.http.get<ApiResultDto<SelectItemDto[]>>(
            `${this.endpoint}/get-codigospostales`, { params }
        );
    }

    getColoniasByCodigoPostal(codigoPostal: string): Observable<ApiResultDto<ColoniaComponentDto>> {
        const params = new HttpParams().set('codigoPostal', codigoPostal);
        return this.http.get<ApiResultDto<ColoniaComponentDto>>(
            `${this.endpoint}/${this.resource}/get-cols-by-cp`, { params }
        );
    }

    getColoniasById(id: number): Observable<ApiResultDto<ColoniaComponentDto>> {
        return this.http.get<ApiResultDto<ColoniaComponentDto>>(
            `${this.endpoint}/${this.resource}/get-cols-by-id/${id}`
        );
    }

    exportar(query?: ColoniaPageQueryDto): Observable<Blob> {
        return this.http.get(`${this.endpoint}/exportar`, {
            params: this.buildParams(query),
            responseType: 'blob',
        });
    }
}
