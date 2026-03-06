import { PageQueryDto } from "../paged-result.dto";

export interface TipoDireccionDto
{
    id: number;
    sTipoDireccion: string;
}

export interface TipoDireccionPageQueryDto extends PageQueryDto {
}

export type CreateTipoDireccionDto = Omit<TipoDireccionDto, 'id'>;
export type UpdateTipoDireccionDto = Partial<Omit<TipoDireccionDto, 'id'>>;
