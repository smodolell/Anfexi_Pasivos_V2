namespace Anfx.Pasivos.Domain.Entities;

public class EstatusContrato
{
    
    public int IdEstatusContrato { get; set; }
    public string EstatusContrato1 { get; set; } = string.Empty;

    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
}
