namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TipoTablaAmortizaTipoPagoCapital
{
    public int IdTipoTablaAmortiza { get; set; }
    public int IdTipoPagoCapital { get; set; }
    public bool Seleccionado { get; set; }

    public virtual PSV_TipoPagoCapital PSV_TipoPagoCapital { get; set; }
    public virtual PSV_TipoTablaAmortiza PSV_TipoTablaAmortiza { get; set; }
}
