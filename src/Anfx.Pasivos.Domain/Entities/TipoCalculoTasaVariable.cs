namespace Anfx.Pasivos.Domain.Entities;

public class TipoCalculoTasaVariable
{

    public int IdTipoCalculoTasaVariable { get; set; }
    public string TipoCalculoTasaVariable1 { get; set; } = string.Empty;
    public string Proceso { get; set; } = string.Empty;

    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
}
