namespace Anfx.Pasivos.Domain.Entities;

public partial class TipoGeneracionComprobante
{
    public TipoGeneracionComprobante()
    {
        this.TipoMovimiento = new HashSet<TipoMovimiento>();
    }

    public int IdTipoGeneracionComprobante { get; set; }
    public string TipoGeneracionComprobante1 { get; set; }

    public virtual ICollection<TipoMovimiento> TipoMovimiento { get; set; }
}
