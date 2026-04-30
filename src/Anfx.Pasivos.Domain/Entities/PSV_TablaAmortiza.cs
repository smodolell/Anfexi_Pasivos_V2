namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TablaAmortiza
{
    public int IdTablaAmortiza { get; set; }
    public int IdTipoTabla { get; set; }
    public int IdContrato { get; set; }
    public DateTime? FecInicial { get; set; }
    public DateTime? FecFinal { get; set; }
    public DateTime? FecVencimiento { get; set; }
    public int? NoPago { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal Seguro { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal SaldoFinal { get; set; }
    public bool? RequiereCalculo { get; set; }
    public int VersionTabla { get; set; }
    public bool Procesado { get; set; }
    public bool EsValorResidual { get; set; }
    public decimal? TasaCalculo { get; set; }
    public virtual PSV_Contrato PSV_Contrato { get; set; } = null!;
}
