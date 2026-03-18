namespace Anfx.Pasivos.Application.Features.Operaciones.DTOs;

public class CargoAdicionalViewDto
{
    public int IdContratoPasivo { get; set; }
    public string Contrato { get; set; } = string.Empty;
    public string TipoCredito { get; set; } = string.Empty;
    public string PSV_EstatusContrato { get; set; } = string.Empty;
    public decimal? CapitalFinanciado { get; set; }
    public string Periodicidad { get; set; } = string.Empty;
    public int? Plazo { get; set; }
    public string TipoMoneda { get; set; } = string.Empty;
    public DateTime? FecInicioContrato { get; set; }
    public DateTime? FecPrimeraRenta { get; set; }
    public DateTime? FecActivacion { get; set; }
    public DateTime? FecFinContrato { get; set; }
    public string Tasa { get; set; }
    public decimal? SaldoInsoluto { get; set; }
    public decimal? TasaIva { get; set; }
    public DateTime? FechaCierre { get; set; }
    public bool? TasaEsVariable { get; set; }
    public string? Fondeador { get; set; }
    public decimal SaldoVencido { get; set; }

    public List<MovimientoItemDto> Movimientos { get; set; } = new List<MovimientoItemDto>();


}
