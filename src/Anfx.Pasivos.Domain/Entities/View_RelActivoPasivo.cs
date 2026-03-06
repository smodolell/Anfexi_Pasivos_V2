namespace Anfx.Pasivos.Domain.Entities;

public partial class View_RelActivoPasivo
{
    public int ID { get; set; }
    public string Contrato { get; set; }
    public Nullable<decimal> Capital { get; set; }
    public string TipoCredito { get; set; }
    public string Fondeador { get; set; }
    public int IdFondeador { get; set; }
    public Nullable<int> IdContratoPasivo { get; set; }
    public int IdContratoPasivoLC { get; set; }
}
