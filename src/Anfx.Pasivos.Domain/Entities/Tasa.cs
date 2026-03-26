namespace Anfx.Pasivos.Domain.Entities;

public class Tasa
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
    public decimal? ValorTasa { get; set; }
    public DateTime? FecTasa { get; set; }
    public bool? EsVariable { get; set; }
    public int? IdFondeador { get; set; }
    public decimal? Spred { get; set; }
    public decimal? RangoMinimo { get; set; }
    public decimal? RangoMaximo { get; set; }
    public int? IdTipoCredito { get; set; }
    public int? IdPlazo { get; set; }
    public decimal? Valor { get; set; }
    public string? CodeDofGob { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<Contrato> Contrato1 { get; set; }
    public virtual TipoCredito TipoCredito { get; set; }
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato1 { get; set; }
}
