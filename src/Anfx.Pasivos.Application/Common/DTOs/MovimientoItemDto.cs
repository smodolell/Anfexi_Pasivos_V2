namespace Anfx.Pasivos.Application.Common.DTOs;

public class MovimientoItemDto
{
    public int IdMovimiento { get; set; }
    public int IdTipoMovimiento { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int NoPago { get; set; }
    public DateTime? FecMovimiento { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal SaldoCapital { get; set; }
    public decimal SaldoInteres { get; set; }
    public decimal SaldoIVA { get; set; }
    public decimal SaldoTotal { get; set; }
    public bool EsRenta { get; set; }

}