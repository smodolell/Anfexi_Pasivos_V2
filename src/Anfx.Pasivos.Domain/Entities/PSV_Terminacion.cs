namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Terminacion
{

    public int IdTerminacion { get; set; }
    public int IdContrato { get; set; }
    public int IdTipoTerminacion { get; set; }
    public int IdTipoReduccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime FechaAnticipo { get; set; }
    public decimal MontoAnticipo { get; set; }
    public decimal? MontoInteres { get; set; }
    public decimal? MontoPena { get; set; }
    public decimal? MontoIVA_Interes { get; set; }
    public decimal? MontoIVA_Pena { get; set; }
    public decimal MontoTotal { get; set; }

    public virtual PSV_Contrato PSV_Contrato { get; set; } =null!;
    public virtual PSV_TipoTerminacion PSV_TipoTerminacion { get; set; } = null!;
    public virtual ICollection<PSV_Movimiento> PSV_Movimiento { get; set; } = new HashSet<PSV_Movimiento>();
}
