namespace Anfx.Pasivos.Domain.Entities;

public class TipoMantenimiento
{

    public int IdTipoMantenimiento { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
}
