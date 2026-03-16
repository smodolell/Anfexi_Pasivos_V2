import { PageQueryDto } from '../paged-result.dto';

export interface TipoCreditoListItemDto {
  id: number;
  tipoCredito: string;
}

export interface TipoCreditoDetailDto {
  id: number;
  tipoCredito: string;
  idTipoTablaAmortiza: number;
  idTipoMovimiento: number;
  idTipoMovimiento_Mora: number;
}

export interface TipoCreditoPageQueryDto extends PageQueryDto {}

export type CreateTipoCreditoDto = Omit<TipoCreditoDetailDto, 'id'>;
export type UpdateTipoCreditoDto = Partial<CreateTipoCreditoDto>;
