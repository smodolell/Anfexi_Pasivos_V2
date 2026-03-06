using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Configuracion.Dtos;

public class TipoCreditoDto 
{
    [StringLength(150, ErrorMessage = "debe ser < que 150 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string TipoCredito { get; set; }
    public int IdTipoMovimiento { get; set; }
    [StringLength(150, ErrorMessage = "debe ser < que 10 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string Prefijo { get; set; } = "";
    [StringLength(150, ErrorMessage = "debe ser < que 10 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string Sufijo { get; set; } = "";
    public int Contador { get; set; }
    public int IdTipoTablaAmortiza { get; set; }
    public bool Activo { get; set; }
    public int IdTipoMovimiento_Mora { get; set; }

}
