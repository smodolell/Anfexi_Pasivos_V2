namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoPago
{

    public int IdTipoPago { get; set; }
    public string TipoPago { get; set; } = string.Empty;

    public virtual ICollection<PSV_Pago> PSV_Pago { get; set; } = new HashSet<PSV_Pago>();
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; } = new HashSet<PSV_TipoTerminacion>();
}
