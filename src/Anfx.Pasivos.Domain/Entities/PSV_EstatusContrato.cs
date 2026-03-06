namespace Anfx.Pasivos.Domain.Entities;

public class PSV_EstatusContrato
{
    public PSV_EstatusContrato()
    {
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
        this.PSV_TipoTerminacion = new HashSet<PSV_TipoTerminacion>();
    }

    public int IdEstatusContrato { get; set; }
    public string EstatusContrato { get; set; }

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; }
}
