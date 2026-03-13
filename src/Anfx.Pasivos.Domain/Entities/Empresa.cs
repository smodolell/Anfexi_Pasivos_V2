namespace Anfx.Pasivos.Domain.Entities;

//public class Empresa
//{
//    public int Id { get; set; }
//    public string sEmpresa { get; set; } = string.Empty;
//    public string RFC { get; set; } = string.Empty;
//    public string RazonSocial { get; set; } = string.Empty;
//    public string Telefono { get; set; } = string.Empty;
//    public string Representante { get; set; } = string.Empty;
//    public string AvisosEstadodeCuenta { get; set; } = string.Empty;
//    public string AdvertenciasEstadodeCuenta { get; set; } = string.Empty;
//    public string AclaracionesEstadodeCuenta { get; set; } = string.Empty;
//    public bool UsaDesembolso { get; set; }
//    public bool Pasivo { get; set; }
//    public int TipoDireccionId { get; set; }
//    public string Calle { get; set; } = string.Empty;
//    public string NumExterior { get; set; } = string.Empty;
//    public string NumInterior { get; set; } = string.Empty;
//    public int ColoniaId { get; set; }
//}

public class Empresa
{
    public int IdEmpresa { get; set; }

    public string? Empresa1 { get; set; }

    public string? RFC { get; set; }

    public string? RazonSocial { get; set; }

    public string? DireccionEmpresa { get; set; }

    public string? Telefono { get; set; }

    public string? Representante { get; set; }

    public string? AvisosEstadodeCuenta { get; set; }

    public string? AdvertenciasEstadodeCuenta { get; set; }

    public string? AclaracionesEstadodeCuenta { get; set; }

    public bool? UsaDesembolso { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<PSV_TipoCredito> PSV_TipoCreditos { get; set; } = new List<PSV_TipoCredito>();
}