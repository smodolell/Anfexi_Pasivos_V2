namespace Anfx.Pasivos.Domain.Entities;

public class SB_TipoMoneda
{

    public int IdTipoMoneda { get; set; }
    public string DescTipoMoneda { get; set; } = string.Empty;
    public string CveCortaTipoMoneda { get; set; } = string.Empty;
    public bool sDefault { get; set; }
    public Nullable<decimal> MontoConvercion { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; } = new HashSet<PSV_LineaCredito>();
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
}
