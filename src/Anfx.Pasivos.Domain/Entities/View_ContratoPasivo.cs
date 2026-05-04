namespace Anfx.Pasivos.Domain.Entities;

public class View_ContratoPasivo
{
    public string Contrato { get; set; } = string.Empty;
    public string Fondeador { get; set; } = string.Empty;
    public string EstatusContrato { get; set; } = string.Empty;
    public decimal? Capital { get; set; }
    public int ID { get; set; }
    public int FondeadorID { get; set; }
    public int? IdEstatusContrato { get; set; }
    public int IdLineaCredito { get; set; }
}
