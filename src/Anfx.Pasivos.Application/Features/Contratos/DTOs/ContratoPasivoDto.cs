
using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

public class ContratoPasivoDto
{
    // Información de Línea de Crédito
    public string Fondeador { get; set; }
    public int IdLineaCredito { get; set; }
    public decimal MaxCapitalDisponible { get; set; }
    public int IdFondeador { get; set; }

    // Datos del Contrato
    public int? IdTipoCredito { get; set; }
    public string Contrato { get; set; }
    public int? IdEstatusContrato { get; set; }

    // Datos Base
    public decimal? CapitalFinanciado { get; set; }
    public int? Plazo { get; set; }
    public int IdMoneda { get; set; }
    public int IdPeriodicidad { get; set; }

    // Tasa Ordinaria
    public bool? TipoTasa { get; set; }
    public int IdTasa { get; set; }
    public decimal? TasaIva { get; set; }
    public decimal? TasaBase { get; set; }
    public decimal? Tasa { get; set; }
    public decimal? PuntosMas { get; set; }
    public decimal? PuntosPor { get; set; }

    // Tasa Mora
    public bool? TipoTasaMora { get; set; }
    public int IdTasaMora { get; set; }
    public decimal? TasaMora { get; set; }
    public decimal? TasaBaseMora { get; set; }
    public decimal? PuntosMasMora { get; set; }
    public decimal? FactorMora { get; set; }

    // Fechas
    public DateTime? FechaFirmaContrato { get; set; }
    public DateTime? FecInicioContrato { get; set; }
    public DateTime? FecPrimeraRenta { get; set; }
    public DateTime? FecActivacion { get; set; }
    public DateTime? FecFinContrato { get; set; }

    // Configuración de Tabla Amortización
    public int IdTipoTablaAmortiza { get; set; }
    public int? IdTipoCapitalizacion { get; set; }
    public int? IdTipoPagoCapital { get; set; }
    public int? IdPeriodicidad_TTA { get; set; }
    public int? NoPagosIrregulares { get; set; }

    // Propiedad auxiliar para mostrar en la vista
    public string TipoCredito { get; set; }

    // Pagos Irregulares
    public List<PagoIrregularDto> Pagos { get; set; }

    // Constructor con valores por defecto
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
    public DateTime? FecVencimiento { get; set; }
}
//using System.ComponentModel.DataAnnotations;

//namespace Anfx.Pasivos.Application.Features.Contratos.DTOs;

//public class ContratoPasivoDto 
//{
//    public int IdContrato { get; set; }
//    public bool? TipoTasaMora { get; set; }
//    public bool? TipoTasa { get; set; }
//    public decimal MaxCapitalDisponible { get; set; }
//    public int IdLineaCredito { get; set; }
//    public string Fondeador { get; set; }
//    public string Contrato { get; set; }
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
//    [Required(ErrorMessage = "Requerido")]
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
//    public DateTime? FechaCierre { get; set; }
//    public bool? TasaEsVariable { get; set; }
//    public int IdFondeador { get; set; }
//    public decimal? FactorFIRA { get; set; }
//    public int IdTipoTablaAmortiza { get; set; }
//    public int? IdPeriodicidad_TTA { get; set; }
//    public int? IdTipoCapitalizacion { get; set; }
//    public int? IdTipoPagoCapital { get; set; }
//    public int? NoPagosIrregulares { get; set; }

//    public string TipoCredito { get; set; }


//    public List<PagoIrregularDto> Pagos { get; set; }
//    public ContratoPasivoDto()
//    {
//        FecInicioContrato = DateTime.Now;
//        IdEstatusContrato = 1;
//        TasaIva = 0.0000m;
//        IdPeriodicidad = 3;
//    }
//}

//public class PagoIrregularDto
//{
//    public int NoPago { get; set; }
//    public decimal Capital { get; set; }
//    [Required(ErrorMessage = " ")]
//    public DateTime? FecVencimiento { get; set; }
//}
