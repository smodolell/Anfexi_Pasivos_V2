namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TipoPagoCapital
{
    public PSV_TipoPagoCapital()
    {
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
        this.Contrato = new HashSet<Contrato>();
        this.PSV_TipoTablaAmortizaTipoPagoCapital = new HashSet<PSV_TipoTablaAmortizaTipoPagoCapital>();
    }

    public int IdTipoPagoCapital { get; set; }
    public string TipoPagoCapital { get; set; }

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<PSV_TipoTablaAmortizaTipoPagoCapital> PSV_TipoTablaAmortizaTipoPagoCapital { get; set; }
}
