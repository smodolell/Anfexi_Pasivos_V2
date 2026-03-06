namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_Fondeador
{
    public PSV_Fondeador()
    {
        this.PSV_Pago = new HashSet<PSV_Pago>();
        this.PSV_LineaCredito = new HashSet<PSV_LineaCredito>();
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
    }

    public int IdFondeador { get; set; }
    public string Fondeador { get; set; }
    public string ClaveCuentaContable { get; set; }

    public virtual ICollection<PSV_Pago> PSV_Pago { get; set; }
    public virtual ICollection<PSV_LineaCredito> PSV_LineaCredito { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
}
