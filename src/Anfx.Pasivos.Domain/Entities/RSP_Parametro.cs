using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anfx.Pasivos.Domain.Entities;

public class RSP_Parametro
{
    public Guid Id { get; set; }

    [Required]
    public int ReporteId { get; set; }

    [Required]
    public int InputId { get; set; }

    [Required]
    [MaxLength(80)]
    public string NomParametro { get; set; } = "";
    [Required]
    [MaxLength(30)]
    public string TipoDato { get; set; } = "";

    //[Required]
    //public bool HayRef { get; set; }

    [Required]
    [MaxLength(60)]
    public string TablaRef { get; set; } = "";

    [Required]
    [MaxLength(60)]
    public string ColumnaValor { get; set; } = "";

    [Required]
    [MaxLength(60)]
    public string ColumnaTexto { get; set; } = "";


    [Required]
    [MaxLength(80)]
    public string Display { get; set; } = "";


    [Required]
    public int Order { get; set; } = 0;


    [ForeignKey(nameof(ReporteId))]
    public RSP_Reporte RSP_Reporte { get; set; } = null!;

    [ForeignKey(nameof(InputId))]
    public RSP_Input RSP_Input { get; set; } = null!;
}
