namespace Anfx.Pasivos.Domain.Entities;

public class PSV_RelLineaCreditoContrato
{
    public int IdLineaCredito { get; set; }
    public int IdContrato { get; set; }

    public virtual PSV_LineaCredito PSV_LineaCredito { get; set; } = null!;
    public virtual PSV_Contrato PSV_Contrato { get; set; } = null!;
}
