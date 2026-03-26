
namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class ContratoPasivoListItem
{
    public string Contrato { get; set; } = string.Empty;
    public string Fondeador { get; set; } = string.Empty;
    public string EstatusContrato { get; set; } = string.Empty;
    public decimal Capital { get; set; }
    public int ID { get; set; }
    public int FondeadorID { get; set; }
    public int IdEstatusContrato { get; set; }
    public int IdLineaCredito { get; set; }
}
