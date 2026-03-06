namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Terminacion
{
    public PSV_Terminacion()
    {
        this.PSV_Movimiento = new HashSet<PSV_Movimiento>();
    }

    public int IdTerminacion { get; set; }
    public int IdContrato { get; set; }
    public int IdTipoTerminacion { get; set; }
    public int IdTipoReduccion { get; set; }
    public System.DateTime FechaRegistro { get; set; }
    public System.DateTime FechaAnticipo { get; set; }
    public decimal MontoAnticipo { get; set; }
    public Nullable<decimal> MontoInteres { get; set; }
    public Nullable<decimal> MontoPena { get; set; }
    public Nullable<decimal> MontoIVA_Interes { get; set; }
    public Nullable<decimal> MontoIVA_Pena { get; set; }
    public decimal MontoTotal { get; set; }

    public virtual PSV_Contrato PSV_Contrato { get; set; }
    public virtual PSV_TipoTerminacion PSV_TipoTerminacion { get; set; }
    public virtual ICollection<PSV_Movimiento> PSV_Movimiento { get; set; }
}
