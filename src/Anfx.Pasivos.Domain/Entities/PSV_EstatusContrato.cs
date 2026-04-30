namespace Anfx.Pasivos.Domain.Entities;

public class PSV_EstatusContrato
{

    public int IdEstatusContrato { get; set; }
    public string EstatusContrato { get; set; } = string.Empty;

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; } = new HashSet<PSV_TipoTerminacion>();
}
