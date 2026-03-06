import { PageQueryDto } from "../paged-result.dto";

export interface RolDto {
    id: number;
    sRol: string;
    descripcion: string;
}

export interface RolPageQueryDto extends PageQueryDto {
    activo?: boolean;
    sortby?: SortingParameter[];
}

export interface SortingParameter {
  column: string;
  desc: boolean; // La propiedad 'desc' determina el tipo de ordenamiento: si su valor es 'true', se aplica un orden descendente; si es 'false', el orden será ascendente.
}

export type CreateRolDto = Omit<RolDto, 'id'>;
export type UpdateRolDto = Partial<Omit<RolDto, 'id'>>;