namespace Anfx.Pasivos.Domain.Entities;

public class PSV_Fondeador
{

    public int IdFondeador { get; set; }
    public string Fondeador { get; set; } = string.Empty;
    public string? ClaveCuentaContable { get; set; }

    public virtual ICollection<PSV_Pago> PSV_Pago { get; set; } = new HashSet<PSV_Pago>();
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; } = new HashSet<PSV_LineaCredito>();
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
}
