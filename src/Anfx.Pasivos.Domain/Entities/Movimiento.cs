namespace Anfx.Pasivos.Domain.Entities;

public class Movimiento
{
    public int IdMovimiento { get; set; }

    public DateTime? FecMovimiento { get; set; }

    public decimal? Capital { get; set; }

    public decimal? Interes { get; set; }

    public decimal? IVA { get; set; }

    public decimal? Total { get; set; }

    public decimal? SaldoCapital { get; set; }

    public decimal? SaldoInteres { get; set; }

    public decimal? SaldoIVA { get; set; }

    public decimal? SaldoTotal { get; set; }

    public DateTime? FecUltimoCambio { get; set; }

    public int? IdContrato { get; set; }

    public virtual Contrato Contrato { get; set; } = null!;

}