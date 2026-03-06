import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_SISTEMA_URL } from '../../api.config';
import { PagedResultDto } from '../../../types/paged-result.dto';
import { ApiResultDto } from '../../../types/apiresult.dto';
import { RolDto, RolPageQueryDto, CreateRolDto, UpdateRolDto } from '../../../types/sistema/rol.dto';

@Injectable({ providedIn: 'root' })
export class RolService {
  private readonly resource = 'roles';

  constructor(
    private http: HttpClient,
    @Inject(API_SISTEMA_URL) private readonly baseUrl: string
  ) {}

  getAll(params?: RolPageQueryDto): Observable<ApiResultDto<PagedResultDto<RolDto>>> {
    const body = params || {};

    return this.http.post<ApiResultDto<PagedResultDto<RolDto>>>(`${this.baseUrl}/${this.resource}/paginados`, body);
  }

  getById(id: number): Observable<ApiResultDto<RolDto>> {
    return this.http.get<ApiResultDto<RolDto>>(`${this.baseUrl}/${this.resource}/${id}`);
  }

  create(dto: CreateRolDto): Observable<ApiResultDto<RolDto>> {
    return this.http.post<ApiResultDto<RolDto>>(`${this.baseUrl}/${this.resource}`, dto);
  }

  update(id: number, dto: UpdateRolDto): Observable<ApiResultDto<RolDto>> {
    return this.http.put<ApiResultDto<RolDto>>(`${this.baseUrl}/${this.resource}/${id}`, dto);
  }

  delete(id: number): Observable<ApiResultDto<void>> {
    return this.http.delete<ApiResultDto<void>>(`${this.baseUrl}/${this.resource}/${id}`);
  }
}
