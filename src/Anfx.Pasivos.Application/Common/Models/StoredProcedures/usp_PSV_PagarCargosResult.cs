using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Common.Models.StoredProcedures;

public class usp_PSV_PagarCargosResult
{
    public string? Error { get; set; }
    public int? IdPago { get; set; }
    public decimal? MontoAplicado { get; set; }
    public decimal? MontoSaldo { get; set; }
}
