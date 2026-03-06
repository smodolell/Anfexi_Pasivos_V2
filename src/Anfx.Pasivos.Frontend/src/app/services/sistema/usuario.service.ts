import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_SISTEMA_URL } from '../../api.config';
import { PagedResultDto, PageQueryDto } from '../../../types/paged-result.dto';
import { ApiResultDto } from '../../../types/apiresult.dto';
import {
  UsuarioDto,
  UsuarioItemDto,
  UsuarioPageQueryDto,
  CreateUsuarioDto,
  UpdateUsuarioDto,
  RolItemDto,
} from '../../../types/sistema/usuario.dto';
import { SelectItemDto } from '../../../types/selectitem.dto';

@Injectable({ providedIn: 'root' })
export class UsuarioService {
  private readonly resource = 'usuarios';

  constructor(
    private http: HttpClient,
    @Inject(API_SISTEMA_URL) private readonly baseUrl: string,
  ) {}

  getAll(params?: UsuarioPageQueryDto): Observable<ApiResultDto<PagedResultDto<UsuarioItemDto>>> {
    let httpParams = new HttpParams();
    if (params) {
      if (params.q) httpParams = httpParams.set('q', params.q);
      if (params.page != null) httpParams = httpParams.set('page', String(params.page));
      if (params.size != null) httpParams = httpParams.set('size', String(params.size));
      if (params.activo != null) httpParams = httpParams.set('activo', String(params.activo));
    }
    return this.http.get<ApiResultDto<PagedResultDto<UsuarioItemDto>>>(
      `${this.baseUrl}/${this.resource}`,
      { params: httpParams },
    );
  }

  getById(id: number): Observable<ApiResultDto<UsuarioDto>> {
    return this.http.get<ApiResultDto<UsuarioDto>>(`${this.baseUrl}/${this.resource}/${id}`);
  }

  getRoles(): Observable<ApiResultDto<RolItemDto[]>> {
    return this.http.get<ApiResultDto<RolItemDto[]>>(`${this.baseUrl}/${this.resource}/roles`);
  }
  getRolesSelectList(): Observable<ApiResultDto<RolItemDto[]>> {
    return this.http.get<ApiResultDto<RolItemDto[]>>(`${this.baseUrl}/roles/select-list`);
  }
  create(dto: CreateUsuarioDto): Observable<ApiResultDto<UsuarioDto>> {
    return this.http.post<ApiResultDto<UsuarioDto>>(`${this.baseUrl}/${this.resource}`, dto);
  }

  update(id: number, dto: UpdateUsuarioDto): Observable<ApiResultDto<UsuarioDto>> {
    return this.http.put<ApiResultDto<UsuarioDto>>(`${this.baseUrl}/${this.resource}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResultDto<void>> {
    return this.http.delete<ApiResultDto<void>>(`${this.baseUrl}/${this.resource}/${id}`);
  }
}
