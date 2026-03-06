using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anfx.Pasivos.Application.Features.Operaciones.DTOs
{
    public class AnticipoCapitalPasivoDto
    {
        public int IdTipoCredito { get; set; }
        public int IdContratoPasivo { get; set; }
        public int IdTipoReduccion { get; set; }
        [Required(ErrorMessage = "Requerido")]
        public int IdTipoTerminacion { get; set; }
        public DateTime FechaAnticipo { get; set; }
        [Range(0.01, 999999999.99, ErrorMessage = "[{1} ~ {2}]")]
        public decimal MontoAnticipo { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal MontoIVA_Interes { get; set; }
        public decimal MontoPena { get; set; }
        public decimal MontoIVA_Pena { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal PorcIVA_Interes { get; set; }
        public decimal PorcIVA_Pena { get; set; }
        public bool EsLiquidacion { get; set; }
        public bool CalculaInteres { get; set; }
        public bool PermitePena { get; set; }
    }

    public class InfoGeneralContratoPasivoDto : AnticipoCapitalPasivoDto
    {
        public string Contrato { get; set; }
        public string TipoCredito { get; set; }
        public string PSV_EstatusContratoEstatusContrato { get; set; }
        public Nullable<decimal> CapitalFinanciado { get; set; }
        public string Periodicidad { get; set; }
        public Nullable<int> Plazo { get; set; }
        public string TipoMoneda { get; set; }
        public Nullable<System.DateTime> FecInicioContrato { get; set; }
        public Nullable<System.DateTime> FecPrimeraRenta { get; set; }
        public Nullable<System.DateTime> FecActivacion { get; set; }
        public Nullable<System.DateTime> FecFinContrato { get; set; }
        public string Tasa { get; set; }
        public Nullable<decimal> SaldoInsoluto { get; set; }
        public Nullable<decimal> TasaIva { get; set; }
        public Nullable<System.DateTime> FechaCierre { get; set; }
        public Nullable<bool> TasaEsVariable { get; set; }
        public string Fondeador { get; set; }
        public Decimal SaldoVencido { get; set; }
        public string EstatusContrato { get; set; }
        public List<TablaAmortizaItemDto> TablaAmortiza { get; set; }
        public List<MovimientoItemDto> Movimientos { get; set; }
        public List<PagoItemDto> Pagos { get; set; }

    }
}
public class PagoItemDto
{
    public int IdPago { get; set; }
    public int IdContratoPasivo { get; set; }
    public string TipoPago { get; set; }
    public string CuentaBancaria { get; set; }
    public DateTime? FecPagoValor { get; set; }
    public DateTime? FecPagoRegistro { get; set; }
    public Decimal MontoPago { get; set; }
    public Decimal MontoAplicado { get; set; }
    public Decimal MontoAplicadoOtros { get; set; }
    public Decimal SaldoPago { get; set; }

}

public class MovimientoItemDto
{
    public int IdMovimiento { get; set; }
    public string Descripcion { get; set; }
    public int NoPago { get; set; }
    public Nullable<System.DateTime> FecMovimiento { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal SaldoCapital { get; set; }
    public decimal SaldoInteres { get; set; }
    public decimal SaldoIVA { get; set; }
    public decimal SaldoTotal { get; set; }
    public bool EsRenta { get; set; }

}

public class TablaAmortizaItemDto
{
    public int IdTablaAmortiza { get; set; }
    public Nullable<System.DateTime> FecInicial { get; set; }
    public Nullable<System.DateTime> FecVencimiento { get; set; }
    public Nullable<int> NoPago { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal Seguro { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public bool Procesado { get; set; }
}