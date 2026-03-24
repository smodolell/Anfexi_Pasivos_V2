namespace Anfx.Pasivos.Application.Common.DTOs;

public class DetallePagoMovimientoDto
{
    public int IdPago { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public string CuentaBancaria { get; set; } = string.Empty;
    public string Contrato { get; set; } = string.Empty;
    public DateTime? FecPagoValor { get; set; }
    public DateTime? FecPagoRegistro { get; set; }
    public decimal MontoPago { get; set; }
    public decimal CapitalPagado { get; set; }
    public decimal InteresPagado { get; set; }
    public decimal IVAPagado { get; set; }
    public decimal TotalPagado { get; set; }
    public bool Estatus { get; set; }
    public string? CausaCancelacion { get; set; } = string.Empty;
}
