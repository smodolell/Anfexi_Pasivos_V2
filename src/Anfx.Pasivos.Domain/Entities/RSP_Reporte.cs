using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Domain.Entities;

public class RSP_Reporte
{
    public int Id { get; set; }


    [Required]
    [MaxLength(80)]
    public string NomReporte { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string StoredProcedure { get; set; } = "";

    public int? ReporteFormatoId { get; set; }



    [Required]
    public bool Activo { get; set; }

    public ICollection<RSP_Parametro> RSP_Parametro { get; set; } = new HashSet<RSP_Parametro>();
    //public ICollection<RSP_ReporteRol> RSP_ReporteRol { get; set; } = new HashSet<RSP_ReporteRol>();


}
