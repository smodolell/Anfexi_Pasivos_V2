import { PageQueryDto } from "../paged-result.dto";
import { SelectItemDto } from "../selectitem.dto";

export interface UsuarioItemDto {
    id: number;
    nombreCompleto: string;
    email: string;
    usuarioNombre: string;
    rolNombre: string;
}
  
export interface UsuarioDto {
    id: number;
    nombreCompleto: string;
    email: string;
    usuarioNombre: string;
    fechaRegistro: Date;
    activo: boolean;
    rolId: number;
}

export interface UsuarioPageQueryDto extends PageQueryDto {
    activo?: boolean;
}

export interface UsuarioFormData extends Partial<UsuarioDto> {
    PermiteEditarContrasenia?: boolean;
    Contrasenia?: string;
    Confirma_Contrasenia? : string;
}

export type CreateUsuarioDto = Omit<UsuarioDto, 'id' | 'fechaRegistro'>;
export type UpdateUsuarioDto = Partial<Omit<UsuarioDto, 'id' | 'fechaRegistro'>>;

export type RolItemDto = SelectItemDto;