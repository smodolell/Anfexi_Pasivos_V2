import { Injectable, inject } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { API_CATALOGO_URL } from '../../api.config';
import { BaseCatalogService } from '../base-catalog.service';
import { PageQueryDto } from '../../../types/paged-result.dto';
import { TipoPagoListItemDto, CreateTipoPagoDto, UpdateTipoPagoDto } from '../../../types/catalogos/tipo-pago.dto';

@Injectable({ providedIn: 'root' })
export class TipoPagoService extends BaseCatalogService<
  TipoPagoListItemDto,
  TipoPagoListItemDto,
  CreateTipoPagoDto,
  UpdateTipoPagoDto
> {
  protected override readonly baseUrl  = inject(API_CATALOGO_URL);
  protected override readonly resource = 'catalogos/tipo-pago';

  protected override buildParams(query?: PageQueryDto): HttpParams {
    let params = new HttpParams();
    if (!query) return params;
    if (query.q)       params = params.set('q',              query.q);
    if (query.page)    params = params.set('page',           String(query.page));
    if (query.size)    params = params.set('size',           String(query.size));
    if (query.sortBy)  params = params.set('sortColumn',     query.sortBy);
    if (query.sortDir) params = params.set('sortDescending', String(query.sortDir === 'desc'));
    return params;
  }
}
