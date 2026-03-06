namespace Anfx.Pasivos.Domain.Entities;

public partial class View_ContratoPasivo
{
    public string Contrato { get; set; }
    public string Fondeador { get; set; }
    public string EstatusContrato { get; set; }
    public Nullable<decimal> Capital { get; set; }
    public int ID { get; set; }
    public int FondeadorID { get; set; }
    public Nullable<int> IdEstatusContrato { get; set; }
    public int IdLineaCredito { get; set; }
}
