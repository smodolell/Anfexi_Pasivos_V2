namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Contrato
{

    public PSV_Contrato()
    {
        this.PSV_ContratoPagoIrregular = new HashSet<PSV_ContratoPagoIrregular>();
        this.PSV_Movimiento = new HashSet<PSV_Movimiento>();
        this.PSV_RelActivoPasivo = new HashSet<PSV_RelActivoPasivo>();
        //this.PSV_LineaCredito = new HashSet<PSV_LineaCredito>();
        this.PSV_TablaAmortiza = new HashSet<PSV_TablaAmortiza>();
        this.PSV_Terminacion = new HashSet<PSV_Terminacion>();
    }

    public int IdContrato { get; set; }
    public string Contrato { get; set; }
    public int? IdTipoCredito { get; set; }
    public int? IdEstatusContrato { get; set; }
    public decimal? Capital { get; set; }
    public decimal? PorcEnganche { get; set; }
    public decimal? Enganche { get; set; }
    public decimal? CapitalFinanciado { get; set; }
    public int IdPeriodicidad { get; set; }
    public int? Plazo { get; set; }
    public int IdMoneda { get; set; }
    public DateTime? FecInicioContrato { get; set; }
    public DateTime? FecPrimeraRenta { get; set; }
    public DateTime? FecActivacion { get; set; }
    public DateTime? FecFinContrato { get; set; }
    public int IdTasa { get; set; }
    public decimal? TasaBase { get; set; }
    public decimal? PuntosMas { get; set; }
    public decimal? PuntosPor { get; set; }
    public decimal? Tasa { get; set; }
    public decimal? TasaBaseMora { get; set; }
    public int IdTasaMora { get; set; }
    public decimal? PuntosMasMora { get; set; }
    public decimal? PuntosPorMora { get; set; }
    public decimal? FactorMora { get; set; }
    public decimal? TasaMora { get; set; }
    public decimal? SaldoInsoluto { get; set; }
    public decimal? BallonPayment { get; set; }
    public decimal? PorcBallonPayment { get; set; }
    public decimal? ValorResidual { get; set; }
    public decimal? PorcValorResidual { get; set; }
    public decimal? DepositoEnGarantia { get; set; }
    public decimal? OpcionDeCompra { get; set; }
    public decimal? PorcOpcionDeCompra { get; set; }
    public decimal? TasaIva { get; set; }
    public int? VersionTabla { get; set; }
    public int? IdTipoCalculoTasaVariable { get; set; }
    public decimal? NroRentasDepositoGarantia { get; set; }
    public DateTime? FechaFirmaContrato { get; set; }
    public int? IdTipoMantenimiento { get; set; }
    public decimal? TasaMensual { get; set; }
    public DateTime? FechaCierre { get; set; }
    public bool? TasaEsVariable { get; set; }
    public int IdFondeador { get; set; }
    public decimal? FactorFIRA { get; set; }
    public int IdTipoTablaAmortiza { get; set; }
    public int? IdPeriodicidad_TTA { get; set; }
    public int? IdTipoPagoCapital { get; set; }
    public int? NoPagosIrregulares { get; set; }
    public int? IdTipoCapitalizacion { get; set; }
    public bool CapturaManualTAPasiva { get; set; }

    public virtual PSV_EstatusContrato PSV_EstatusContrato { get; set; }
    public virtual PSV_Fondeador PSV_Fondeador { get; set; }
    public virtual PSV_TipoCapitalizacion PSV_TipoCapitalizacion { get; set; }
    public virtual PSV_TipoCredito PSV_TipoCredito { get; set; }
    public virtual PSV_TipoPagoCapital PSV_TipoPagoCapital { get; set; }
    public virtual SB_Periodicidad SB_Periodicidad { get; set; }
    public virtual SB_Periodicidad SB_Periodicidad1 { get; set; }
    public virtual SB_TipoMoneda SB_TipoMoneda { get; set; }
    public virtual Tasa Tasa1 { get; set; }
    public virtual Tasa Tasa2 { get; set; }
    public virtual ICollection<PSV_ContratoPagoIrregular> PSV_ContratoPagoIrregular { get; set; }
    public virtual ICollection<PSV_Movimiento> PSV_Movimiento { get; set; }
    public virtual ICollection<PSV_RelActivoPasivo> PSV_RelActivoPasivo { get; set; }
    //public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; }
    public virtual PSV_TipoTablaAmortiza PSV_TipoTablaAmortiza { get; set; }
    public virtual ICollection<PSV_TablaAmortiza> PSV_TablaAmortiza { get; set; }
    public virtual ICollection<PSV_Terminacion> PSV_Terminacion { get; set; }

    public virtual ICollection<PSV_RelLineaCreditoContrato> PSV_RelLineaCreditoContrato { get; set; }
}