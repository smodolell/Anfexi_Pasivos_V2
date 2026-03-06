import { PageQueryDto } from "../paged-result.dto";
import { SelectItemDto } from "../selectitem.dto";

export interface ColoniaDto
{
    id: number;
    sColonia: string;
    estado: string;
    municipio: string;
    codigoPostal: string;
}

export interface ColoniaPageQueryDto extends PageQueryDto {
}

export type CreateColoniaDto = Omit<ColoniaDto, 'id'>;
export type UpdateColoniaDto = Partial<Omit<ColoniaDto, 'id'>>;

export interface ColoniaComponentDto {
    codigoPostal: string;
    estado: string;
    municipio: string;
    colonias: SelectItemDto[];
}