namespace Anfx.Pasivos.Domain.Entities;

public partial class TipoMantenimiento
{
    public TipoMantenimiento()
    {
        this.Contrato = new HashSet<Contrato>();
    }

    public int IdTipoMantenimiento { get; set; }
    public string Clave { get; set; }
    public string Descripcion { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
}
