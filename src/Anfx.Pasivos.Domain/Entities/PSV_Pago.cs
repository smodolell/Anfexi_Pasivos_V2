namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Pago
{
   

    public int IdPago { get; set; }
    public int IdTipoPago { get; set; }
    public int IdCuentaBancaria { get; set; }
    public string Contrato { get; set; } = string.Empty;
    public DateTime? FecPagoRegistro { get; set; }
    public DateTime? FecPagoValor { get; set; }
    public decimal MontoPago { get; set; }
    public decimal MontoPagoAplicado { get; set; }
    public decimal SaldoPago { get; set; }
    public bool Suspenso { get; set; }
    public bool Estatus { get; set; }
    public DateTime? FecUltimoCambio { get; set; }
    public int? IdFondeador { get; set; }

    public virtual PSV_Fondeador? PSV_Fondeador { get; set; }
    public virtual ICollection<PSV_RelPagoMovimiento> PSV_RelPagoMovimiento { get; set; } = new HashSet<PSV_RelPagoMovimiento>();
    public virtual PSV_CuentaBancaria PSV_CuentaBancaria { get; set; } = null!;
    public virtual PSV_TipoPago PSV_TipoPago { get; set; } = null!;
}
