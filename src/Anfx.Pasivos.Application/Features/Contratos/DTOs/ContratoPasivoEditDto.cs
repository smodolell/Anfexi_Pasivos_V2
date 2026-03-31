
namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class ContratoPasivoEditDto
{


    #region Informacion

    public string Fondeador { get; set; } =string.Empty;

    public int IdLineaCredito { get; set; }
    public int IdEstatusContrato { get; set; }
    public decimal MaxCapitalDisponible { get; set; }
    public string Contrato { get; set; } = string.Empty;
    public string LineaCredito { get; set; } = string.Empty;
    public string EstatusContrato { get; set; } = string.Empty;



    #endregion





    public bool? TipoTasa { get; set; }

    public int IdTipoCredito { get; set; }
    //public decimal Capital { get; set; }
    //public decimal PorcEnganche { get; set; }
    //public decimal Enganche { get; set; }
    public decimal CapitalFinanciado { get; set; }
    public int IdPeriodicidad { get; set; }
    public int Plazo { get; set; }
    public int IdMoneda { get; set; }
    public DateTime? FecInicioContrato { get; set; }
    public DateTime FecPrimeraRenta { get; set; }
    public DateTime FecActivacion { get; set; }
    public DateTime FecFinContrato { get; set; }
    public int IdTasa { get; set; }
    public decimal TasaBase { get; set; }
    public decimal PuntosMas { get; set; }
    public decimal PuntosPor { get; set; }
    public decimal Tasa { get; set; }
    public decimal TasaBaseMora { get; set; }
    public int IdTasaMora { get; set; }
    public decimal PuntosMasMora { get; set; }
    public decimal PuntosPorMora { get; set; }
    public decimal FactorMora { get; set; }
    public decimal TasaMora { get; set; }
    //public decimal SaldoInsoluto { get; set; }
    //public decimal BallonPayment { get; set; }
    //public decimal PorcBallonPayment { get; set; }
    //public decimal ValorResidual { get; set; }
    //public decimal PorcValorResidual { get; set; }
    //public decimal DepositoEnGarantia { get; set; }
    //public decimal OpcionDeCompra { get; set; }
    //public decimal PorcOpcionDeCompra { get; set; }
    public decimal TasaIva { get; set; }
    public int VersionTabla { get; set; }
    public int IdTipoCalculoTasaVariable { get; set; }
    public decimal NroRentasDepositoGarantia { get; set; }
    public DateTime? FechaFirmaContrato { get; set; }
    public int IdTipoMantenimiento { get; set; }
    public decimal TasaMensual { get; set; }
    public DateTime? FechaCierre { get; set; }
    public bool TasaEsVariable { get; set; }
    public int IdFondeador { get; set; }
    public decimal FactorFIRA { get; set; }
    public int IdTipoTablaAmortiza { get; set; }
    public int IdPeriodicidad_TTA { get; set; }
    public int? IdTipoCapitalizacion { get; set; }
    public int? IdTipoPagoCapital { get; set; }
    public int? NoPagosIrregulares { get; set; }
    public List<PagoIrregularDto> Pagos { get; set; } = new List<PagoIrregularDto>();
}


//Capital - No aparece en ningún control del formulario

//PorcEnganche - No aparece

//Enganche - No aparece

//SaldoInsoluto - No aparece

//BallonPayment - No aparece

//PorcBallonPayment - No aparece

//ValorResidual - No aparece

//PorcValorResidual - No aparece

//DepositoEnGarantia - No aparece

//OpcionDeCompra - No aparece

//PorcOpcionDeCompra - No aparece

//VersionTabla - No aparece

//IdTipoCalculoTasaVariable - No aparece

//NroRentasDepositoGarantia - No aparece

//IdTipoMantenimiento - No aparece

//TasaMensual - No aparece

//FechaCierre - No aparece

//TasaEsVariable - No aparece

//FactorFIRA - No aparece

//PuntosPorMora - No aparece(aunque FactorMora sí aparece, PuntosPorMora no)

//PuntosMas - Aparece pero está en la vista(sí se usa)

//PuntosPor - Aparece pero está en la vista(sí se usa)