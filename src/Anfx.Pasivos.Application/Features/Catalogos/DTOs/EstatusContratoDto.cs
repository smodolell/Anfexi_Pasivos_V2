using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class EstatusContratoDto
{
    [StringLength(150, ErrorMessage = "debe ser < que 150 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string EstatusContrato { get; set; } = string.Empty;
}
