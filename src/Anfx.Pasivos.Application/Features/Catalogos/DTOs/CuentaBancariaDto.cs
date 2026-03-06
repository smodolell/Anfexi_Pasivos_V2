using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Catalogos.DTOs;

public class CuentaBancariaDto
{
    [Required(ErrorMessage = "Requerido")]
    public int IdBanco { get; set; }

    [StringLength(50, ErrorMessage = "debe ser < que 50 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string CuentaBancaria { get; set; } = string.Empty;

    [StringLength(18, ErrorMessage = "debe ser < que 18 Car.")]
    [Required(ErrorMessage = "Requerido")]
    public string CLABE { get; set; } = string.Empty;
}
