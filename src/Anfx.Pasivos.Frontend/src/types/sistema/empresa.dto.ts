import { PageQueryDto } from "../paged-result.dto";
import { SelectItemDto } from "../selectitem.dto";

export interface EmpresaDto {
    id: number;
    sEmpresa: string;
    rFC: string;
    razonSocial: string;
    telefono: string;
    representante: string;
    avisosEstadodeCuenta: string;
    advertenciasEstadodeCuenta: string;
    aclaracionesEstadodeCuenta: string;
    usaDesembolso: boolean;
    pasivo: boolean;
    tipoDireccionId: number;
    calle: string;
    numExterior: string;
    numInterior: string;
    coloniaId: number;
}

export interface EmpresaPageQueryDto extends PageQueryDto {
}

export type CreateEmpresaDto = Omit<EmpresaDto, 'id'>;
export type UpdateEmpresaDto = Partial<Omit<EmpresaDto, 'id'>>;

export type TipoDireccionItemDto = SelectItemDto;
