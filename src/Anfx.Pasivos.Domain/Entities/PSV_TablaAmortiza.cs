namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TablaAmortiza
{
    public int IdTablaAmortiza { get; set; }
    public int IdTipoTabla { get; set; }
    public int IdContrato { get; set; }
    public Nullable<System.DateTime> FecInicial { get; set; }
    public Nullable<System.DateTime> FecFinal { get; set; }
    public Nullable<System.DateTime> FecVencimiento { get; set; }
    public Nullable<int> NoPago { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
    public decimal Seguro { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    public decimal SaldoFinal { get; set; }
    public Nullable<bool> RequiereCalculo { get; set; }
    public int VersionTabla { get; set; }
    public bool Procesado { get; set; }
    public bool EsValorResidual { get; set; }
    public Nullable<decimal> TasaCalculo { get; set; }

    public virtual PSV_Contrato PSV_Contrato { get; set; }
}
