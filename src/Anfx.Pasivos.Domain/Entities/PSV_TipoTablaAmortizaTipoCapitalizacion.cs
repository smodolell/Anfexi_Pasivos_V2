namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoTablaAmortizaTipoCapitalizacion
{
    public int IdTipoTablaAmortiza { get; set; }
    public int IdTipoCapitalizacion { get; set; }
    public bool Seleccionado { get; set; }

    public virtual PSV_TipoCapitalizacion PSV_TipoCapitalizacion { get; set; } = null!;
    public virtual PSV_TipoTablaAmortiza PSV_TipoTablaAmortiza { get; set; } = null!;
}
