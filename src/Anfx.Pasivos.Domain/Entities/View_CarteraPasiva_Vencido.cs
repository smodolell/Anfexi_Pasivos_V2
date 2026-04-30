namespace Anfx.Pasivos.Domain.Entities;

public class View_CarteraPasiva_Vencido
{
    public string ContratoPasivo { get; set; } = string.Empty;
    public DateTime? FecVencimiento { get; set; }
    public decimal? Capital { get; set; }
    public decimal? Interes { get; set; }
    public decimal? Total { get; set; }
    public int IdFondeador { get; set; }
    public int IdContratoPasivo { get; set; }
    public int? IdContratoActivo { get; set; }
    public decimal? SaldoTotal { get; set; }
}
