namespace Anfx.Pasivos.Domain.Entities;

public class View_CarteraPasiva_PorVencerV2
{
    public string ContratoPasivo { get; set; } = string.Empty;
    public DateTime? FecVencimiento { get; set; }
    public decimal? Capital { get; set; }
    public decimal? Interes { get; set; }
    public decimal? Total { get; set; }
    public string SaldoTotal { get; set; } = string.Empty;
    public int IdFondeador { get; set; }
    public int IdContratoPasivo { get; set; }
}
