namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Pago
{
    public PSV_Pago()
    {
        this.PSV_RelPagoMovimiento = new HashSet<PSV_RelPagoMovimiento>();
    }

    public int IdPago { get; set; }
    public int IdTipoPago { get; set; }
    public int IdCuentaBancaria { get; set; }
    public string Contrato { get; set; }
    public Nullable<System.DateTime> FecPagoRegistro { get; set; }
    public Nullable<System.DateTime> FecPagoValor { get; set; }
    public decimal MontoPago { get; set; }
    public decimal MontoPagoAplicado { get; set; }
    public decimal SaldoPago { get; set; }
    public bool Suspenso { get; set; }
    public bool Estatus { get; set; }
    public Nullable<System.DateTime> FecUltimoCambio { get; set; }
    public Nullable<int> IdFondeador { get; set; }

    public virtual PSV_Fondeador PSV_Fondeador { get; set; }
    public virtual ICollection<PSV_RelPagoMovimiento> PSV_RelPagoMovimiento { get; set; }
    public virtual PSV_CuentaBancaria PSV_CuentaBancaria { get; set; }
    public virtual PSV_TipoPago PSV_TipoPago { get; set; }
}
