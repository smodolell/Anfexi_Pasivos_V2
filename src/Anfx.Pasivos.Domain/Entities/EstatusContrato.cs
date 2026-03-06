namespace Anfx.Pasivos.Domain.Entities;

public partial class EstatusContrato
{
    public EstatusContrato()
    {
        this.Contrato = new HashSet<Contrato>();
    }

    public int IdEstatusContrato { get; set; }
    public string EstatusContrato1 { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
}
