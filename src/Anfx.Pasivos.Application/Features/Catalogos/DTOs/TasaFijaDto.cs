using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class TasaFijaDto
{
    [Required(ErrorMessage = "Requerido")]
    [StringLength(100, ErrorMessage = "debe ser < que 100 Car.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Requerido")]
    public decimal ValorTasa { get; set; }

    public DateTime? FecTasa { get; set; }
}
