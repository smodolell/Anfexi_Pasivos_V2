namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TipoPago
{
    public PSV_TipoPago()
    {
        this.PSV_Pago = new HashSet<PSV_Pago>();
        this.PSV_TipoTerminacion = new HashSet<PSV_TipoTerminacion>();
    }

    public int IdTipoPago { get; set; }
    public string TipoPago { get; set; }

    public virtual ICollection<PSV_Pago> PSV_Pago { get; set; }
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; }
}
