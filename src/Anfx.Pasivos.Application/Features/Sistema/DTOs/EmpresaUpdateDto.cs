namespace Anfx.Pasivos.Application.Features.Sistema.DTOs;

public class EmpresaUpdateDto
{
    public int Id { get; set; }
    public string sEmpresa { get; set; } = string.Empty;
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Telefono { get; set; }  
    public string? Representante { get; set; }
    public string? AvisosEstadodeCuenta { get; set; }
    public string? AdvertenciasEstadodeCuenta { get; set; }
    public string? AclaracionesEstadodeCuenta { get; set; }
    public bool UsaDesembolso { get; set; }
    //public bool Pasivo { get; set; }
    //public int TipoDireccionId { get; set; }
    //public string? Calle { get; set; }
    //public string? NumExterior { get; set; }
    //public string? NumInterior { get; set; }
    //public int ColoniaId { get; set; }
}