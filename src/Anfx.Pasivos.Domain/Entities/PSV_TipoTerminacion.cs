namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoTerminacion
{
    public PSV_TipoTerminacion()
    {
        this.PSV_TipoCredito = new HashSet<PSV_TipoCredito>();
        this.PSV_Terminacion = new HashSet<PSV_Terminacion>();
    }

    public int IdTipoTerminacion { get; set; }
    public string TipoTerminacion { get; set; }
    public int IdTipoMovimientoBaja { get; set; }
    public Nullable<int> IdTipoMovimientoPena { get; set; }
    public Nullable<int> IdTipoMovimientoInteres { get; set; }
    public bool PermiteUsarDeposito { get; set; }
    public Nullable<int> IdCuentaBancariaDeposito { get; set; }
    public Nullable<int> IdTipoPagoDeposito { get; set; }
    public bool SumaInteresSigAmortizacion { get; set; }
    public bool PermiteCalculoInteres { get; set; }
    public bool EsLiquidacionTotal { get; set; }
    public bool EsPorcAnticipo_PenaAnticipo { get; set; }
    public Nullable<decimal> PorcAnticipo_PenaAnticipo { get; set; }
    public bool EsDiasVencidos { get; set; }
    public Nullable<int> DiasVencidos { get; set; }
    public Nullable<int> IdEstatusContratoTerminacion { get; set; }

    public virtual PSV_CuentaBancaria PSV_CuentaBancaria { get; set; }
    public virtual PSV_EstatusContrato PSV_EstatusContrato { get; set; }
    public virtual PSV_TipoPago PSV_TipoPago { get; set; }
    public virtual TipoMovimiento TipoMovimiento { get; set; }
    public virtual TipoMovimiento TipoMovimiento1 { get; set; }
    public virtual TipoMovimiento TipoMovimiento2 { get; set; }
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito { get; set; }
    public virtual ICollection<PSV_Terminacion> PSV_Terminacion { get; set; }
}
