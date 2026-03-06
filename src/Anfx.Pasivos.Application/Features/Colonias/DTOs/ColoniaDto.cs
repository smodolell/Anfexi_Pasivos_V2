using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Colonias.DTOs
{
    public class ColoniaDto
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        
        [DisplayName("Colonia")]
        [Required(ErrorMessage = "El nombre de la colonia es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre de la colonia no puede exceder 200 caracteres")]
        public string sColonia { get; set; } = string.Empty;
        
        [DisplayName("Estado")]
        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(200, ErrorMessage = "El estado no puede exceder 200 caracteres")]
        public string Estado { get; set; } = string.Empty;
        
        [DisplayName("Municipio")]
        [Required(ErrorMessage = "El municipio es requerido")]
        [MaxLength(200, ErrorMessage = "El municipio no puede exceder 200 caracteres")]
        public string Municipio { get; set; } = string.Empty;
        
        [DisplayName("Código Postal")]
        [Required(ErrorMessage = "El código postal es requerido")]
        [MaxLength(10, ErrorMessage = "El código postal no puede exceder 10 caracteres")]
        public string CodigoPostal { get; set; } = string.Empty;
    }

    public class CreateColoniaDto
    {
        [Required(ErrorMessage = "El nombre de la colonia es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre de la colonia no puede exceder 200 caracteres")]
        public string sColonia { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(200, ErrorMessage = "El estado no puede exceder 200 caracteres")]
        public string Estado { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El municipio es requerido")]
        [MaxLength(200, ErrorMessage = "El municipio no puede exceder 200 caracteres")]
        public string Municipio { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El código postal es requerido")]
        [MaxLength(10, ErrorMessage = "El código postal no puede exceder 10 caracteres")]
        public string CodigoPostal { get; set; } = string.Empty;
    }

    public class UpdateColoniaDto
    {
        [Required(ErrorMessage = "El nombre de la colonia es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre de la colonia no puede exceder 200 caracteres")]
        public string sColonia { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El estado es requerido")]
        [MaxLength(200, ErrorMessage = "El estado no puede exceder 200 caracteres")]
        public string Estado { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El municipio es requerido")]
        [MaxLength(200, ErrorMessage = "El municipio no puede exceder 200 caracteres")]
        public string Municipio { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El código postal es requerido")]
        [MaxLength(10, ErrorMessage = "El código postal no puede exceder 10 caracteres")]
        public string CodigoPostal { get; set; } = string.Empty;
    }

    public class ColoniaComponentDto
    {
        public string Estado { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public ICollection<SelectItemDto> Colonias { get; set; } = new List<SelectItemDto>();
    }
}
