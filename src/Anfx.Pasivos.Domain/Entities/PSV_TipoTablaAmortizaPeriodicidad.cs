namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TipoTablaAmortizaPeriodicidad
{
    public int IdTipoTablaAmortiza { get; set; }
    public int IdPeriodicidad { get; set; }
    public bool Seleccionado { get; set; }

    public virtual SB_Periodicidad SB_Periodicidad { get; set; }
    public virtual PSV_TipoTablaAmortiza PSV_TipoTablaAmortiza { get; set; }
}
