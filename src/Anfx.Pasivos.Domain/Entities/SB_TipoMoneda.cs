namespace Anfx.Pasivos.Domain.Entities;

public partial class SB_TipoMoneda
{
    public SB_TipoMoneda()
    {
        this.Contrato = new HashSet<Contrato>();
        this.PSV_LineaCredito = new HashSet<PSV_LineaCredito>();
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
    }

    public int IdTipoMoneda { get; set; }
    public string DescTipoMoneda { get; set; }
    public string CveCortaTipoMoneda { get; set; }
    public bool sDefault { get; set; }
    public Nullable<decimal> MontoConvercion { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
}
