namespace Anfx.Pasivos.Domain.Entities;

public class TipoCredito
{

    public int IdTipoCredito { get; set; }
    public string ClaveTipoCredito { get; set; } = string.Empty;
    public string TipoCredito1 { get; set; } = string.Empty;

    public bool SeUsaEnPasivos { get; set; }
    public Nullable<bool> Estatus { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();

    public virtual ICollection<PSV_RelLineaCreditoTipoCredito> PSV_RelLineaCreditoTipoCredito { get; set; } = new HashSet<PSV_RelLineaCreditoTipoCredito>();
}
