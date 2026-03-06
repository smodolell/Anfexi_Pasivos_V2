namespace Anfx.Pasivos.Domain.Entities;

public class Empresa
{
    public int Id { get; set; }
    public string sEmpresa { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Representante { get; set; } = string.Empty;
    public string AvisosEstadodeCuenta { get; set; } = string.Empty;
    public string AdvertenciasEstadodeCuenta { get; set; } = string.Empty;
    public string AclaracionesEstadodeCuenta { get; set; } = string.Empty;
    public bool UsaDesembolso { get; set; }
    public bool Pasivo { get; set; }
    public int TipoDireccionId { get; set; }
    public string Calle { get; set; } = string.Empty;
    public string NumExterior { get; set; } = string.Empty;
    public string NumInterior { get; set; } = string.Empty;
    public int ColoniaId { get; set; }
}