//using System.Diagnostics.Contracts;

//namespace Anfx.Pasivos.Domain.Entities;

//public partial class Contrato
//{
//    public Contrato()
//    {
//        this.PSV_RelActivoPasivo = new HashSet<PSV_RelActivoPasivo>();
//    }

//    public int IdContrato { get; set; }
//    public string Contrato1 { get; set; }
//    public int? IdPersona { get; set; }
//    public int? IdTipoCredito { get; set; }
//    public int? IdEstatusContrato { get; set; }
//    public decimal? Capital { get; set; }
//    public decimal? PorcEnganche { get; set; }
//    public decimal? Enganche { get; set; }
//    public decimal? CapitalFinanciado { get; set; }
//    public int IdPeriodicidad { get; set; }
//    public int? Plazo { get; set; }
//    public int IdMoneda { get; set; }
//    public DateTime? FecInicioContrato { get; set; }
//    public DateTime? FecPrimeraRenta { get; set; }
//    public DateTime? FecActivacion { get; set; }
//    public DateTime? FecFinContrato { get; set; }
//    public int IdTasa { get; set; }
//    public decimal? TasaBase { get; set; }
//    public decimal? PuntosMas { get; set; }
//    public decimal? PuntosPor { get; set; }
//    public decimal? Tasa { get; set; }
//    public decimal? TasaBaseMora { get; set; }
//    public int IdTasaMora { get; set; }
//    public decimal? PuntosMasMora { get; set; }
//    public decimal? PuntosPorMora { get; set; }
//    public decimal? FactorMora { get; set; }
//    public decimal? TasaMora { get; set; }
//    public decimal? SaldoInsoluto { get; set; }
//    public decimal? BallonPayment { get; set; }
//    public decimal? PorcBallonPayment { get; set; }
//    public decimal? ValorResidual { get; set; }
//    public decimal? PorcValorResidual { get; set; }
//    public decimal? DepositoEnGarantia { get; set; }
//    public decimal? OpcionDeCompra { get; set; }
//    public decimal? PorcOpcionDeCompra { get; set; }
//    public decimal? TasaIva { get; set; }
//    public int? VersionTabla { get; set; }
//    public int? IdTipoCalculoTasaVariable { get; set; }
//    public decimal? NroRentasDepositoGarantia { get; set; }
//    public DateTime? FechaFirmaContrato { get; set; }
//    public int? IdTipoMantenimiento { get; set; }
//    public decimal? TasaMensual { get; set; }
//    public int? IdUsuario { get; set; }
//    public int? IdSucursal { get; set; }
//    public int? IdReestructura { get; set; }
//    public int? IdSector { get; set; }
//    public int? IdSubSector { get; set; }
//    public bool? EsWriteOff { get; set; }
//    public DateTime? FechaCierre { get; set; }
//    public int? IdCotizador { get; set; }
//    public bool? TasaEsVariable { get; set; }
//    public string GraciaCapital { get; set; }
//    public string GraciaInteres { get; set; }
//    public decimal? FactorFIRA { get; set; }
//    public int? IdCredito { get; set; }
//    public int? NumeroPagosCapital { get; set; }
//    public int? IdTipoTablaAmortiza { get; set; }
//    public int? IdPeriodicidad_TTA { get; set; }
//    public int? IdTipoPagoCapital { get; set; }
//    public int? NoPagosIrregulares { get; set; }
//    public int? IdTipoCapitalizacion { get; set; }
//    public bool Emproblemado { get; set; }
//    public string InversionPrincipal { get; set; }
//    public int IdPeriodicidadTC { get; set; }
//    public bool CapturaManualTAPasiva { get; set; }

//    public virtual EstatusContrato EstatusContrato { get; set; }

//    public virtual SB_TipoMoneda SB_TipoMoneda { get; set; }
//    public virtual Tasa Tasa1 { get; set; }
//    public virtual Tasa Tasa2 { get; set; }
//    public virtual TipoCalculoTasaVariable TipoCalculoTasaVariable { get; set; }
//    public virtual TipoMantenimiento TipoMantenimiento { get; set; }
//    public virtual ICollection<PSV_RelActivoPasivo> PSV_RelActivoPasivo { get; set; }
//    public virtual TipoCredito TipoCredito { get; set; }
//    public virtual PSV_TipoCapitalizacion PSV_TipoCapitalizacion { get; set; }
//    public virtual PSV_TipoPagoCapital PSV_TipoPagoCapital { get; set; }
//    public virtual PSV_TipoTablaAmortiza PSV_TipoTablaAmortiza { get; set; }


//    public virtual SB_Periodicidad SB_Periodicidad { get; set; }
//    public virtual SB_Periodicidad SB_Periodicidad1 { get; set; }
//    public virtual SB_Periodicidad SB_Periodicidad2 { get; set; }
//}


using Anfx.Pasivos.Domain.Entities;

public partial class PSV_ContratoMinistracione
{
    public int IdContratoMinistraciones { get; set; }

    public int? IdContrato { get; set; }

    public int? IdFondeador { get; set; }

    public int? IdCuentaDeposito { get; set; }

    public decimal? MontoDeposito { get; set; }

    public DateTime? FechaFondeo { get; set; }

    public int? IdCuentaDispercion { get; set; }

    public decimal? MontoDispersion { get; set; }

    public DateTime? FechaDispersion { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; }

    public virtual PSV_FondeadoresCuenta IdCuentaDepositoNavigation { get; set; }

    public virtual Cat_CuentasDispersion IdCuentaDispercionNavigation { get; set; }

    public virtual PSV_Fondeador IdFondeadorNavigation { get; set; }
}
