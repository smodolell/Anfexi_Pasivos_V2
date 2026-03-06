namespace Anfx.Pasivos.Domain.Entities;

public partial class View_CarteraActiva_Vencido
{
    public string Contrato { get; set; }
    public Nullable<System.DateTime> FecVencimiento { get; set; }
    public Nullable<decimal> Capital { get; set; }
    public Nullable<decimal> Interes { get; set; }
    public Nullable<decimal> Total { get; set; }
    public int IdFondeador { get; set; }
    public int IdContratoPasivo { get; set; }
    public int IdContratoActivo { get; set; }
    public Nullable<decimal> SaldoTotal { get; set; }
}
