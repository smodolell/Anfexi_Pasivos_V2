import { PageQueryDto } from '../paged-result.dto';

export interface TipoPagoListItemDto {
  id: number;
  tipoPago: string;
}

export interface TipoPagoPageQueryDto extends PageQueryDto {}

export type CreateTipoPagoDto = Omit<TipoPagoListItemDto, 'id'>;
export type UpdateTipoPagoDto = Partial<CreateTipoPagoDto>;
