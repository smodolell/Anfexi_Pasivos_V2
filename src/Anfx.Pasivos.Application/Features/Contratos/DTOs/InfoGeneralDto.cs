namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class InfoGeneralContratoPasivoDto
{
    public string Contrato { get; set; }
    public string TipoCredito { get; set; }
    public string PSV_EstatusContratoEstatusContrato { get; set; }
    public Nullable<decimal> CapitalFinanciado { get; set; }
    public string Periodicidad { get; set; }
    public Nullable<int> Plazo { get; set; }
    public string TipoMoneda { get; set; }
    public Nullable<System.DateTime> FecInicioContrato { get; set; }
    public Nullable<System.DateTime> FecPrimeraRenta { get; set; }
    public Nullable<System.DateTime> FecActivacion { get; set; }
    public Nullable<System.DateTime> FecFinContrato { get; set; }
    public string Tasa { get; set; }
    public Nullable<decimal> SaldoInsoluto { get; set; }
    public Nullable<decimal> TasaIva { get; set; }
    public Nullable<System.DateTime> FechaCierre { get; set; }
    public Nullable<bool> TasaEsVariable { get; set; }
    public string Fondeador { get; set; }
    public Decimal SaldoVencido { get; set; }
    public string EstatusContrato { get; set; }
    public List<TablaAmortizaItemDto> TablaAmortiza { get; set; }
    public List<MovimientoItemDto> Movimientos { get; set; }
    public List<PagoItemDto> Pagos { get; set; }

}