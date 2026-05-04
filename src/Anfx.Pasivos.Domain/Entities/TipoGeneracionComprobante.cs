namespace Anfx.Pasivos.Domain.Entities;

public partial class TipoGeneracionComprobante
{

    public int IdTipoGeneracionComprobante { get; set; }
    public string TipoGeneracionComprobante1 { get; set; } = string.Empty;

    public virtual ICollection<TipoMovimiento> TipoMovimiento { get; set; } = new HashSet<TipoMovimiento>();
}
