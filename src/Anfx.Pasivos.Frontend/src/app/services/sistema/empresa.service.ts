import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../api.config';
import { PagedResultDto } from '../../../types/paged-result.dto';
import { ApiResultDto } from '../../../types/apiresult.dto';
import { EmpresaDto, EmpresaPageQueryDto, CreateEmpresaDto, UpdateEmpresaDto, TipoDireccionItemDto } from '../../../types/sistema/empresa.dto';

@Injectable({ providedIn: 'root' })
export class EmpresaService {
  private readonly resource = 'sistema/empresa';
  protected readonly baseUrl = inject(API_BASE_URL);

  constructor(
    // eslint-disable-next-line @angular-eslint/prefer-inject
    private http: HttpClient,
  ) {}

  getAll(params?: EmpresaPageQueryDto): Observable<ApiResultDto<PagedResultDto<EmpresaDto>>> {
    let httpParams = new HttpParams();
    if (params) {
      if (params.q)                 httpParams = httpParams.set('q', params.q);
      if (params.page != null)      httpParams = httpParams.set('page', String(params.page));
      if (params.size != null)      httpParams = httpParams.set('size', String(params.size));
    }
    return this.http.get<ApiResultDto<PagedResultDto<EmpresaDto>>>(`${this.baseUrl}/${this.resource}`, { params: httpParams });
  }

  getById(id: number): Observable<ApiResultDto<EmpresaDto>> {
    return this.http.get<ApiResultDto<EmpresaDto>>(`${this.baseUrl}/${this.resource}/${id}`);
  }

  create(dto: CreateEmpresaDto): Observable<ApiResultDto<EmpresaDto>> {
    return this.http.post<ApiResultDto<EmpresaDto>>(`${this.baseUrl}/${this.resource}`, dto);
  }

  update(id: number, dto: UpdateEmpresaDto): Observable<ApiResultDto<EmpresaDto>> {
    return this.http.put<ApiResultDto<EmpresaDto>>(`${this.baseUrl}/${this.resource}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResultDto<void>> {
    return this.http.delete<ApiResultDto<void>>(`${this.baseUrl}/${this.resource}/${id}`);
  }

  getTiposDirecciones(): Observable<ApiResultDto<TipoDireccionItemDto[]>> {
    return this.http.get<ApiResultDto<TipoDireccionItemDto[]>>(`${this.baseUrl}/${this.resource}/get-tiposdirecciones`);
  }

}
