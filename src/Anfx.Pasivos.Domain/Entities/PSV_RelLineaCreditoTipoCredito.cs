namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_RelLineaCreditoTipoCredito
{
    public int IdLineaCredito { get; set; }
    public int IdTipoCredito { get; set; }
    public bool Seleccionado { get; set; }

    public virtual PSV_LineaCredito PSV_LineaCredito { get; set; }
    public virtual TipoCredito TipoCredito { get; set; }
}
