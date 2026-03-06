namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Movimiento
{
    public PSV_Movimiento()
    {
        this.PSV_RelPagoMovimiento = new HashSet<PSV_RelPagoMovimiento>();
        this.PSV_Terminacion = new HashSet<PSV_Terminacion>();
    }

    public int IdMovimiento { get; set; }
    public int IdTipoMovimiento { get; set; }
    public string Descripcion { get; set; }
    public int NoPago { get; set; }
    public Nullable<System.DateTime> FecMovimiento { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal SaldoCapital { get; set; }
    public decimal SaldoInteres { get; set; }
    public decimal SaldoIVA { get; set; }
    public decimal SaldoTotal { get; set; }
    public Nullable<System.DateTime> FecUltimoCambio { get; set; }
    public Nullable<int> IdContrato { get; set; }
    public Nullable<int> IdFondeador { get; set; }

    public virtual TipoMovimiento TipoMovimiento { get; set; }
    public virtual ICollection<PSV_RelPagoMovimiento> PSV_RelPagoMovimiento { get; set; }
    public virtual PSV_Contrato PSV_Contrato { get; set; }
    public virtual ICollection<PSV_Terminacion> PSV_Terminacion { get; set; }
}
