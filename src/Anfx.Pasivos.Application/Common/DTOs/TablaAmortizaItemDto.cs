namespace Anfx.Pasivos.Application.Common.DTOs;

public class TablaAmortizaItemDto
{
    public int IdTablaAmortiza { get; set; }
    public DateTime? FecInicial { get; set; }
    public DateTime? FecVencimiento { get; set; }
    public int? NoPago { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal Seguro { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public bool Procesado { get; set; }
}
