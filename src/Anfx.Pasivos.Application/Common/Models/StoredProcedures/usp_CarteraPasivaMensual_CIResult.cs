using System.ComponentModel.DataAnnotations.Schema;

namespace Anfx.Pasivos.Application.Common.Models.StoredProcedures;

public partial class usp_CarteraPasivaMensual_CIResult
{
    public int? Id { get; set; }
    public DateOnly? FecIni { get; set; }
    public DateOnly? FecFin { get; set; }
    [Column("Capital", TypeName = "decimal(18,2)")]
    public decimal Capital { get; set; }
    [Column("Interes", TypeName = "decimal(18,2)")]
    public decimal Interes { get; set; }
}
