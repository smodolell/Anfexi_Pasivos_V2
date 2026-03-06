using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TipoPagoDto
{
    [StringLength(150, ErrorMessage = "debe ser < que 150 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string TipoPago { get; set; } = string.Empty;
}
