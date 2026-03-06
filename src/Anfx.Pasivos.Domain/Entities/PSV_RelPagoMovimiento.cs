namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_RelPagoMovimiento
{
    public int IdPagoMovimiento { get; set; }
    public int IdPago { get; set; }
    public int IdMovimiento { get; set; }
    public Nullable<System.DateTime> FecAplicacion { get; set; }
    public decimal CapitalPagado { get; set; }
    public decimal InteresPagado { get; set; }
    public decimal IVAPagado { get; set; }
    public decimal TotalPagado { get; set; }
    public bool Estatus { get; set; }
    public Nullable<System.DateTime> FecUltimoCambio { get; set; }
    public Nullable<System.DateTime> FecCancelacion { get; set; }
    public bool Reaplicado { get; set; }
    public string CausaCancelacion { get; set; }

    public virtual PSV_Movimiento PSV_Movimiento { get; set; }
    public virtual PSV_Pago PSV_Pago { get; set; }
}
