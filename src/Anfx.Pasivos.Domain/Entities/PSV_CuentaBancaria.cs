namespace Anfx.Pasivos.Domain.Entities;

public class PSV_CuentaBancaria
{

    public int IdCuentaBancaria { get; set; }
    public int IdBanco { get; set; }
    public string CuentaBancaria { get; set; }
    public string CLABE { get; set; }

    public virtual ICollection<PSV_Pago> PSV_Pago { get; set; } = new HashSet<PSV_Pago>();
    public virtual PSV_Banco PSV_Banco { get; set; }
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; } = new HashSet<PSV_TipoTerminacion>();
}
