namespace Anfx.Pasivos.Application.Common.DTOs;

public class PagoItemDto
{
    public int IdPago { get; set; }
    public int IdContratoPasivo { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public string CuentaBancaria { get; set; } =string.Empty;
    public DateTime? FecPagoValor { get; set; }
    public DateTime? FecPagoRegistro { get; set; }
    public decimal MontoPago { get; set; }
    public decimal MontoAplicado { get; set; }
    public decimal MontoAplicadoOtros { get; set; }
    public decimal SaldoPago { get; set; }

}
