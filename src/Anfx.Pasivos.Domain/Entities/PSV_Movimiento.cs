namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Movimiento
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
    public DateTime? FecUltimoCambio { get; set; }
    public int? IdContrato { get; set; }
    public int? IdFondeador { get; set; }

    public virtual TipoMovimiento TipoMovimiento { get; set; } = null!;
    public virtual ICollection<PSV_RelPagoMovimiento> PSV_RelPagoMovimiento { get; set; } = new HashSet<PSV_RelPagoMovimiento>();
    public virtual PSV_Contrato PSV_Contrato { get; set; } = null!;
    public virtual ICollection<PSV_Terminacion> PSV_Terminacion { get; set; } = new HashSet<PSV_Terminacion>();
}
