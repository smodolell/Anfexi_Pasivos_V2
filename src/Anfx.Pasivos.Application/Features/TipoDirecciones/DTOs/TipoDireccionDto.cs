using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs
{
    public class TipoDireccionDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El tipo de dirección es requerido")]
        [MaxLength(200, ErrorMessage = "El tipo de dirección no puede exceder 200 caracteres")]
        public string sTipoDireccion { get; set; } = string.Empty;
    }

    public class CreateTipoDireccionDto
    {
        [Required(ErrorMessage = "El tipo de dirección es requerido")]
        [MaxLength(200, ErrorMessage = "El tipo de dirección no puede exceder 200 caracteres")]
        public string sTipoDireccion { get; set; } = string.Empty;
    }

    public class UpdateTipoDireccionDto
    {
        [Required(ErrorMessage = "El tipo de dirección es requerido")]
        [MaxLength(200, ErrorMessage = "El tipo de dirección no puede exceder 200 caracteres")]
        public string sTipoDireccion { get; set; } = string.Empty;
    }
}
