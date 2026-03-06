using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class ContratoPasivoDto 
{
    public bool? TipoTasaMora { get; set; }
    public bool? TipoTasa { get; set; }
    public decimal MaxCapitalDisponible { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public int IdLineaCredito { get; set; }
    public string Fondeador { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public string Contrato { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public Nullable<int> IdTipoCredito { get; set; }
    public Nullable<int> IdEstatusContrato { get; set; }
    public Nullable<decimal> Capital { get; set; }
    public Nullable<decimal> PorcEnganche { get; set; }
    public Nullable<decimal> Enganche { get; set; }
    [Range(0.01, 999999999.99, ErrorMessage = "> 0")]
    public Nullable<decimal> CapitalFinanciado { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public int IdPeriodicidad { get; set; }
    [Range(1, 540, ErrorMessage = "> 0")]
    public Nullable<int> Plazo { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public int IdMoneda { get; set; }
    public Nullable<System.DateTime> FecInicioContrato { get; set; }
    public Nullable<System.DateTime> FecPrimeraRenta { get; set; }
    public Nullable<System.DateTime> FecActivacion { get; set; }
    public Nullable<System.DateTime> FecFinContrato { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public int IdTasa { get; set; }
    public Nullable<decimal> TasaBase { get; set; }
    public Nullable<decimal> PuntosMas { get; set; }
    public Nullable<decimal> PuntosPor { get; set; }
    public Nullable<decimal> Tasa { get; set; }
    public Nullable<decimal> TasaBaseMora { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public int IdTasaMora { get; set; }
    public Nullable<decimal> PuntosMasMora { get; set; }
    public Nullable<decimal> PuntosPorMora { get; set; }
    public Nullable<decimal> FactorMora { get; set; }
    public Nullable<decimal> TasaMora { get; set; }
    public Nullable<decimal> SaldoInsoluto { get; set; }
    public Nullable<decimal> BallonPayment { get; set; }
    public Nullable<decimal> PorcBallonPayment { get; set; }
    public Nullable<decimal> ValorResidual { get; set; }
    public Nullable<decimal> PorcValorResidual { get; set; }
    public Nullable<decimal> DepositoEnGarantia { get; set; }
    public Nullable<decimal> OpcionDeCompra { get; set; }
    public Nullable<decimal> PorcOpcionDeCompra { get; set; }
    public Nullable<decimal> TasaIva { get; set; }
    public Nullable<int> VersionTabla { get; set; }
    public Nullable<int> IdTipoCalculoTasaVariable { get; set; }
    public Nullable<decimal> NroRentasDepositoGarantia { get; set; }
    public Nullable<System.DateTime> FechaFirmaContrato { get; set; }
    public Nullable<int> IdTipoMantenimiento { get; set; }
    public Nullable<decimal> TasaMensual { get; set; }
    public Nullable<System.DateTime> FechaCierre { get; set; }
    public Nullable<bool> TasaEsVariable { get; set; }
    public int IdFondeador { get; set; }
    public Nullable<decimal> FactorFIRA { get; set; }
    [Required(ErrorMessage = "Requerido")]
    public int IdTipoTablaAmortiza { get; set; }
    public Nullable<int> IdPeriodicidad_TTA { get; set; }
    public int? IdTipoCapitalizacion { get; set; }
    public int? IdTipoPagoCapital { get; set; }
    public int? NoPagosIrregulares { get; set; }
    public List<PagoIrregularDto> Pagos { get; set; }
    public ContratoPasivoDto()
    {
        FecInicioContrato = DateTime.Now;
        IdEstatusContrato = 1;
        TasaIva = 0.0000m;
        IdPeriodicidad = 3;
    }
}

public class PagoIrregularDto
{
    public int NoPago { get; set; }
    public decimal Capital { get; set; }
    [Required(ErrorMessage = " ")]
    public System.DateTime? FecVencimiento { get; set; }
}
