namespace Anfx.Pasivos.Domain.Entities;

public class TipoMovimiento
{
    public int IdTipoMovimiento { get; set; }
    public string TipoMovimiento1 { get; set; } = string.Empty;
    public string ClaveTipoMovimiento { get; set; } = string.Empty;
    public bool? GeneraIVACapital { get; set; }
    public bool? GeneraMora { get; set; }
    public bool? Capturable { get; set; }
    public bool? EsRenta { get; set; }
    public bool? Estatus { get; set; }
    public decimal? Orden { get; set; }
    public bool? GeneraIVAInteres { get; set; }
    public int? IdTipoGeneracionComprobante { get; set; }
    public bool? SeparaComprobante { get; set; }
    public bool? CalificaCarteraVencida { get; set; }
    public bool? GeneraCapital { get; set; }
    public bool? GeneraInteres { get; set; }
    public bool? GeneraFees { get; set; }

    public virtual TipoGeneracionComprobante? TipoGeneracionComprobante { get; set; }
    public virtual ICollection<PSV_Movimiento> PSV_Movimiento { get; set; } = new HashSet<PSV_Movimiento>();
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito { get; set; } = new HashSet<PSV_TipoCredito>();
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito1 { get; set; } = new HashSet<PSV_TipoCredito>();
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; } = new HashSet<PSV_TipoTerminacion>();
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion1 { get; set; } = new HashSet<PSV_TipoTerminacion>();
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion2 { get; set; } = new HashSet<PSV_TipoTerminacion>();
}
