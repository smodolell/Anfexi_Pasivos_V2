using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anfx.Pasivos.Domain.Entities;

public class RSP_Archivo
{
    public Guid Id { get; set; }
    public int ReporteId { get; set; }

    public string LogParameters { get; set; } = "";

    [Required]
    public DateTime FechaCreacion { get; set; }

    [Required]
    [MaxLength(200)]
    public string NombreArchivo { get; set; } = "";

    
    [Required]
    [MaxLength(200)]
    public string NombreUnico { get; set; } = "";

    [Required]
    [MaxLength(200)]
    public string ContentType { get; set; } = "";

    [Required]
    [MaxLength(150)]
    public string Extension { get; set; } = "";

    [Required]
    [MaxLength(150)]
    public string MapPath { get; set; } = "";


    [ForeignKey(nameof(ReporteId))]
    public RSP_Reporte RSP_Reporte { get; set; } = null!;

}
