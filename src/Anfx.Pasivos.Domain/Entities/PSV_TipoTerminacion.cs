namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoTerminacion
{

    public int IdTipoTerminacion { get; set; }
    public string TipoTerminacion { get; set; } = string.Empty;
    public int IdTipoMovimientoBaja { get; set; }
    public int? IdTipoMovimientoPena { get; set; }
    public int? IdTipoMovimientoInteres { get; set; }
    public bool PermiteUsarDeposito { get; set; }
    public int? IdCuentaBancariaDeposito { get; set; }
    public int? IdTipoPagoDeposito { get; set; }
    public bool SumaInteresSigAmortizacion { get; set; }
    public bool PermiteCalculoInteres { get; set; }
    public bool EsLiquidacionTotal { get; set; }
    public bool EsPorcAnticipo_PenaAnticipo { get; set; }
    public decimal? PorcAnticipo_PenaAnticipo { get; set; }
    public bool EsDiasVencidos { get; set; }
    public int? DiasVencidos { get; set; }
    public int? IdEstatusContratoTerminacion { get; set; }

    public virtual PSV_CuentaBancaria? PSV_CuentaBancaria { get; set; } 
    public virtual PSV_EstatusContrato? PSV_EstatusContrato { get; set; }
    public virtual PSV_TipoPago? PSV_TipoPago { get; set; }
    public virtual TipoMovimiento? TipoMovimiento { get; set; }
    public virtual TipoMovimiento? TipoMovimiento1 { get; set; }
    public virtual TipoMovimiento? TipoMovimiento2 { get; set; }
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito { get; set; } = new HashSet<PSV_TipoCredito>();
    public virtual ICollection<PSV_Terminacion> PSV_Terminacion { get; set; } = new HashSet<PSV_Terminacion>();
}
