namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoCapitalizacion
{

    public int IdTipoCapitalizacion { get; set; }
    public string TipoCapitalizacion { get; set; } = string.Empty;

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
    public virtual ICollection<PSV_TipoTablaAmortizaTipoCapitalizacion> PSV_TipoTablaAmortizaTipoCapitalizacion { get; set; } = new HashSet<PSV_TipoTablaAmortizaTipoCapitalizacion>();
}
