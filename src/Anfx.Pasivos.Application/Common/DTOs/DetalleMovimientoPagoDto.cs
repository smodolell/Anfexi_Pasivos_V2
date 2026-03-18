namespace Anfx.Pasivos.Application.Common.DTOs;

public class DetalleMovimientoPagoDto
{
    public int IdMovimiento { get; set; }
    public string? Fondeador { get; set; }
    public string? Contrato { get; set; }
    public int NoPago { get; set; }
    public string? Descripcion { get; set; }
    public DateTime? FecMovimiento { get; set; }
    public decimal CapitalPagado { get; set; }
    public decimal InteresPagado { get; set; }
    public decimal IVAPagado { get; set; }
    public decimal TotalPagado { get; set; }
    public bool Estatus { get; set; }
}
