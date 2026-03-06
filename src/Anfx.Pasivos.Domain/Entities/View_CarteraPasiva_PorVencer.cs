namespace Anfx.Pasivos.Domain.Entities;

public partial class View_CarteraPasiva_PorVencer
{
    public string ContratoPasivo { get; set; }
    public Nullable<System.DateTime> FecVencimiento { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal Total { get; set; }
    public int IdFondeador { get; set; }
    public int IdContratoPasivo { get; set; }
    public Nullable<int> IdContratoActivo { get; set; }
    public string SaldoTotal { get; set; }
}
