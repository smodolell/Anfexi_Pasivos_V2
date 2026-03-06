using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Empresas.DTOs;

public class EmpresaCreateDto
{
    [Required(ErrorMessage = "El nombre de la empresa es requerido")]
    [StringLength(180, ErrorMessage = "El nombre no puede exceder 180 caracteres")]
    public string sEmpresa { get; set; } = string.Empty;

    [Required(ErrorMessage = "El RFC es requerido")]
    [StringLength(13, ErrorMessage = "El RFC no puede exceder 13 caracteres")]
    public string RFC { get; set; } = string.Empty;

    [Required(ErrorMessage = "La razón social es requerida")]
    [StringLength(180, ErrorMessage = "La razón social no puede exceder 180 caracteres")]
    public string RazonSocial { get; set; } = string.Empty;

    [StringLength(12, ErrorMessage = "El teléfono no puede exceder 12 caracteres")]
    public string Telefono { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "El representante no puede exceder 150 caracteres")]
    public string Representante { get; set; } = string.Empty;

    public string AvisosEstadodeCuenta { get; set; } = string.Empty;
    public string AdvertenciasEstadodeCuenta { get; set; } = string.Empty;
    public string AclaracionesEstadodeCuenta { get; set; } = string.Empty;
    public bool UsaDesembolso { get; set; }
    public bool Pasivo { get; set; }

    [Required(ErrorMessage = "El tipo de dirección es requerido")]
    public int TipoDireccionId { get; set; }

    [StringLength(200, ErrorMessage = "La calle no puede exceder 200 caracteres")]
    public string Calle { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El número exterior no puede exceder 20 caracteres")]
    public string NumExterior { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El número interior no puede exceder 20 caracteres")]
    public string NumInterior { get; set; } = string.Empty;

    [Required(ErrorMessage = "La colonia es requerida")]
    public int ColoniaId { get; set; }
}
