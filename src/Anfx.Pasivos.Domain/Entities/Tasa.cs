namespace Anfx.Pasivos.Domain.Entities;

public partial class Tasa
{
    public Tasa()
    {
        this.Contrato = new HashSet<Contrato>();
        this.Contrato1 = new HashSet<Contrato>();
        this.PSV_LineaCredito = new HashSet<PSV_LineaCredito>();
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
        this.PSV_Contrato1 = new HashSet<PSV_Contrato>();
    }

    public int IdTasa { get; set; }
    public string Tasa1 { get; set; }
    public Nullable<decimal> ValorTasa { get; set; }
    public Nullable<System.DateTime> FecTasa { get; set; }
    public Nullable<bool> EsVariable { get; set; }
    public Nullable<int> IdFondeador { get; set; }
    public Nullable<decimal> Spred { get; set; }
    public Nullable<decimal> RangoMinimo { get; set; }
    public Nullable<decimal> RangoMaximo { get; set; }
    public Nullable<int> IdTipoCredito { get; set; }
    public Nullable<int> IdPlazo { get; set; }
    public Nullable<decimal> Valor { get; set; }
    public string CodeDofGob { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<Contrato> Contrato1 { get; set; }
    public virtual TipoCredito TipoCredito { get; set; }
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato1 { get; set; }
}
