namespace Anfx.Pasivos.Domain.Entities;

public class PSV_RelLineaCreditoTipoCredito
{
    public int IdLineaCredito { get; set; }
    public int IdTipoCredito { get; set; }
    public bool Seleccionado { get; set; }

    public virtual PSV_LineaCredito PSV_LineaCredito { get; set; } = null!;
    public virtual TipoCredito TipoCredito { get; set; } = null!;
}
