namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoPagoCapital
{

    public int IdTipoPagoCapital { get; set; }
    public string TipoPagoCapital { get; set; } = string.Empty;
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
    public virtual ICollection<PSV_TipoTablaAmortizaTipoPagoCapital> PSV_TipoTablaAmortizaTipoPagoCapital { get; set; } = new HashSet<PSV_TipoTablaAmortizaTipoPagoCapital>();

}
