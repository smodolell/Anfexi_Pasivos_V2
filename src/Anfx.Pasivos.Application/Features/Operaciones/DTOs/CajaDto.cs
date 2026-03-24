using Anfx.Pasivos.Application.Features.Contratos.Queries;

namespace Anfx.Pasivos.Application.Features.Operaciones.DTOs;

public class CajaDto
{
    public string ContratoPasivo { get; set; }
    public string Fondeador { get; set; }
    public int? IdFondeador { get; set; }
    public int? IdContrato { get; set; }
    public int? IdUsuario { get; set; }
    public int IdTipoPago { get; set; }
    public int IdBanco { get; set; }
    public int IdCuentaBancaria { get; set; }
    public DateOnly FechaPago { get; set; }
    public string? Referencia { get; set; }
    public decimal MontoPago { get; set; }
    public List<MovimientoPagoItem> Movimientos { get; set; } = new List<MovimientoPagoItem>();

}


public class MovimientoPagoItem
{
    public int ID { get; set; }
    public int NoPago { get; set; }
    public DateTime? FecMovimiento { get; set; }
    public string? Descripcion { get; set; }
    public decimal SaldoCapital { get; set; }
    public decimal SaldoInteres { get; set; }
    public decimal SaldoIVA { get; set; }
    public decimal SaldoTotal { get; set; }
    public bool Seleccionado { get; set; }
}
