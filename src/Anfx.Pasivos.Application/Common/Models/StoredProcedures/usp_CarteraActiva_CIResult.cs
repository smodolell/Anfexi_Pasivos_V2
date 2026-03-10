using System.ComponentModel.DataAnnotations.Schema;

namespace Anfx.Pasivos.Application.Common.Models.StoredProcedures
{
    public partial class usp_CarteraActiva_CIResult
    {
        [Column("Capital", TypeName = "decimal(38,2)")]
        public decimal? Capital { get; set; }
        [Column("Interes", TypeName = "decimal(38,2)")]
        public decimal? Interes { get; set; }
    }
}
