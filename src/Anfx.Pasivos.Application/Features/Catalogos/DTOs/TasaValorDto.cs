using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaValorDto
{
    [Required(ErrorMessage = "Requerido")]
    public decimal ValorTasa { get; set; }

    [Required(ErrorMessage = "Requerido")]
    public DateTime FecValorTasa { get; set; }

    public DateTime? FecRegistroTasa { get; set; }
}
