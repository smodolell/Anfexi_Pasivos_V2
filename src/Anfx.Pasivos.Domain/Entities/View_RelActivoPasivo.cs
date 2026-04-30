namespace Anfx.Pasivos.Domain.Entities;

public class View_RelActivoPasivo
{
    public int ID { get; set; }
    public string Contrato { get; set; } = string.Empty;
    public decimal? Capital { get; set; }
    public string TipoCredito { get; set; } = string.Empty;
    public string Fondeador { get; set; } = string.Empty;
    public int IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int IdContratoPasivoLC { get; set; }
}
