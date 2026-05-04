using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaVariableDto
{
    [Required(ErrorMessage = "Requerido")]
    [StringLength(100, ErrorMessage = "debe ser < que 100 Car.")]
    public string Nombre { get; set; } = string.Empty;
}
