using Anfx.Pasivos.Application.Features.Procesos.Commands;

namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class InfoGeneralContratoPasivoDto
{
    public int IdContrato { get; set; }
    public string Contrato { get; set; } = string.Empty;
    public string TipoCredito { get; set; } = string.Empty;
    public string PSV_EstatusContratoEstatusContrato { get; set; } = string.Empty;
    public decimal? CapitalFinanciado { get; set; }
    public string Periodicidad { get; set; } = string.Empty;
    public int? Plazo { get; set; }
    public string TipoMoneda { get; set; } = string.Empty;
    public DateTime? FecInicioContrato { get; set; }
    public DateTime? FecPrimeraRenta { get; set; }
    public DateTime? FecActivacion { get; set; }
    public DateTime? FecFinContrato { get; set; }
    public string Tasa { get; set; } = string.Empty;
    public decimal? SaldoInsoluto { get; set; }
    public decimal? TasaIva { get; set; }
    public DateTime? FechaCierre { get; set; }
    public bool? TasaEsVariable { get; set; }
    public string Fondeador { get; set; } = string.Empty;
    public decimal SaldoVencido { get; set; }
    public string EstatusContrato { get; set; } = string.Empty;
    public List<TablaAmortizaItemDto> TablaAmortiza { get; set; } = new();
    public List<MovimientoItemDto> Movimientos { get; set; } = new();
    public List<PagoItemDto> Pagos { get; set; } = new();

}