namespace Anfx.Pasivos.Domain.Entities;

public partial class TipoCredito
{
    public TipoCredito()
    {
        this.Contrato = new HashSet<Contrato>();
        this.Tasa = new HashSet<Tasa>();
        this.PSV_RelLineaCreditoTipoCredito = new HashSet<PSV_RelLineaCreditoTipoCredito>();
    }

    public int IdTipoCredito { get; set; }
    public string ClaveTipoCredito { get; set; }
    public string TipoCredito1 { get; set; }
    public Nullable<int> IdTipoMovimiento { get; set; }
    public Nullable<bool> PermiteEnganche { get; set; }
    public Nullable<bool> CausaPenaPrepago { get; set; }
    public Nullable<bool> PermiteBallonPayment { get; set; }
    public Nullable<bool> ManejaActivos { get; set; }
    public Nullable<bool> PermiteDepositoEnGarantia { get; set; }
    public Nullable<bool> PermiteOpcionDeCompra { get; set; }
    public Nullable<bool> ActivosConIvaSumanCapital { get; set; }
    public Nullable<bool> CalculaIvaTasaReal { get; set; }
    public Nullable<bool> PermiteValorResidual { get; set; }
    public string Prefijo { get; set; }
    public string Postfijo { get; set; }
    public Nullable<int> Consecutivo { get; set; }
    public Nullable<int> IdTipoMovimientoEnganche { get; set; }
    public Nullable<int> IdTipoMovimientoBallon { get; set; }
    public Nullable<int> IdTipoMovimientoValorResidual { get; set; }
    public Nullable<int> IdTipoMovimientoDeposito { get; set; }
    public Nullable<int> IdTipoMovimientoOpcion { get; set; }
    public Nullable<int> IdEmpresa { get; set; }
    public int EsquemaFinanciero { get; set; }
    public Nullable<decimal> TasaMoraDefault { get; set; }
    public Nullable<int> IdTipoMovimientoMora { get; set; }
    public Nullable<int> IdTipoMovimientoGastos { get; set; }
    public Nullable<decimal> FactorGastoCob { get; set; }
    public Nullable<decimal> MaxMontoGastoCob { get; set; }
    public Nullable<int> PeriodoGracia { get; set; }
    public Nullable<int> IdTipoMovimientoCancelCheque { get; set; }
    public Nullable<decimal> PorcCancelacionCheque { get; set; }
    public Nullable<decimal> MontoPenaNoDevuelto { get; set; }
    public Nullable<int> IdTipoMovimientoNoDevuelto { get; set; }
    public Nullable<int> IdMonedaNoDevuelto { get; set; }
    public Nullable<int> MesesDePenaNoDevuelto { get; set; }
    public Nullable<decimal> MontoKilometraje { get; set; }
    public Nullable<int> IdTipoMonedaKm { get; set; }
    public Nullable<decimal> CargoCheque { get; set; }
    public Nullable<bool> AplicaGraciaCapital { get; set; }
    public Nullable<int> GraciaCapital { get; set; }
    public Nullable<decimal> CATMinimo { get; set; }
    public Nullable<decimal> PorcComApertura { get; set; }
    public Nullable<decimal> Gastoslegales { get; set; }
    public Nullable<decimal> GastosdeRegistro { get; set; }
    public Nullable<bool> PermiteSubsidio { get; set; }
    public Nullable<bool> PermiteCreditoGrupal { get; set; }
    public Nullable<int> IdTipoTablaAmortiza { get; set; }
    public Nullable<bool> PermiteGastosAdministracion { get; set; }
    public Nullable<int> IdTipoProducto { get; set; }
    public Nullable<decimal> FactorTasaMora { get; set; }
    public bool PermiteLimiteMontoCredito { get; set; }
    public Nullable<decimal> MontoMinimo { get; set; }
    public Nullable<decimal> MontoMaximo { get; set; }
    public bool PermiteSeguroEnPeriodo { get; set; }
    public Nullable<int> IdTipoMovimientoSeguro { get; set; }
    public decimal PorcSeguroEnPeriodo { get; set; }
    public bool PermiteAforo { get; set; }
    public Nullable<decimal> PorcAforo { get; set; }
    public bool PermiteGraciaCapitalInteres { get; set; }
    public decimal FactorCapadicadDePago { get; set; }
    public bool PermiteTasaVariable { get; set; }
    public bool PermiteUsarPAyFactor { get; set; }
    public bool SeUsaEnPasivos { get; set; }
    public Nullable<bool> Estatus { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual Empresa Empresa { get; set; }
    public virtual ICollection<Tasa> Tasa { get; set; }
    public virtual TipoMovimiento TipoMovimiento { get; set; }
    public virtual TipoMovimiento TipoMovimiento1 { get; set; }
    public virtual TipoMovimiento TipoMovimiento2 { get; set; }
    public virtual TipoMovimiento TipoMovimiento3 { get; set; }
    public virtual TipoMovimiento TipoMovimiento4 { get; set; }
    public virtual TipoMovimiento TipoMovimiento5 { get; set; }
    public virtual TipoMovimiento TipoMovimiento6 { get; set; }
    public virtual TipoMovimiento TipoMovimiento7 { get; set; }
    public virtual TipoMovimiento TipoMovimiento8 { get; set; }
    public virtual TipoMovimiento TipoMovimiento9 { get; set; }
    public virtual TipoMovimiento TipoMovimiento10 { get; set; }
    public virtual ICollection<PSV_RelLineaCreditoTipoCredito> PSV_RelLineaCreditoTipoCredito { get; set; }
}
