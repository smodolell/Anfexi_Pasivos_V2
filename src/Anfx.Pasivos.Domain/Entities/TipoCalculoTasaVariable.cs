namespace Anfx.Pasivos.Domain.Entities;

public partial class TipoCalculoTasaVariable
{
    public TipoCalculoTasaVariable()
    {
        this.Contrato = new HashSet<Contrato>();
    }

    public int IdTipoCalculoTasaVariable { get; set; }
    public string TipoCalculoTasaVariable1 { get; set; }
    public string Proceso { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
}
