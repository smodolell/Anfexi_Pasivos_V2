namespace Anfx.Pasivos.Domain.Entities;

public partial class TipoMovimiento
{
    public TipoMovimiento()
    {
        this.PSV_Movimiento = new HashSet<PSV_Movimiento>();
        this.TipoCredito = new HashSet<TipoCredito>();
        this.TipoCredito1 = new HashSet<TipoCredito>();
        this.TipoCredito2 = new HashSet<TipoCredito>();
        this.TipoCredito3 = new HashSet<TipoCredito>();
        this.TipoCredito4 = new HashSet<TipoCredito>();
        this.TipoCredito5 = new HashSet<TipoCredito>();
        this.TipoCredito6 = new HashSet<TipoCredito>();
        this.TipoCredito7 = new HashSet<TipoCredito>();
        this.TipoCredito8 = new HashSet<TipoCredito>();
        this.TipoCredito9 = new HashSet<TipoCredito>();
        this.TipoCredito10 = new HashSet<TipoCredito>();
        this.PSV_TipoCredito = new HashSet<PSV_TipoCredito>();
        this.PSV_TipoCredito1 = new HashSet<PSV_TipoCredito>();
        this.PSV_TipoTerminacion = new HashSet<PSV_TipoTerminacion>();
        this.PSV_TipoTerminacion1 = new HashSet<PSV_TipoTerminacion>();
        this.PSV_TipoTerminacion2 = new HashSet<PSV_TipoTerminacion>();
    }

    public int IdTipoMovimiento { get; set; }
    public string TipoMovimiento1 { get; set; }
    public string ClaveTipoMovimiento { get; set; }
    public Nullable<bool> GeneraIVACapital { get; set; }
    public Nullable<bool> GeneraMora { get; set; }
    public Nullable<bool> Capturable { get; set; }
    public Nullable<bool> EsRenta { get; set; }
    public Nullable<bool> Estatus { get; set; }
    public Nullable<decimal> Orden { get; set; }
    public Nullable<bool> GeneraIVAInteres { get; set; }
    public Nullable<int> IdTipoGeneracionComprobante { get; set; }
    public Nullable<bool> SeparaComprobante { get; set; }
    public Nullable<bool> CalificaCarteraVencida { get; set; }
    public Nullable<bool> GeneraCapital { get; set; }
    public Nullable<bool> GeneraInteres { get; set; }
    public Nullable<bool> GeneraFees { get; set; }

    public virtual TipoGeneracionComprobante TipoGeneracionComprobante { get; set; }
    public virtual ICollection<PSV_Movimiento> PSV_Movimiento { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito1 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito2 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito3 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito4 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito5 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito6 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito7 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito8 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito9 { get; set; }
    public virtual ICollection<TipoCredito> TipoCredito10 { get; set; }
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito { get; set; }
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito1 { get; set; }
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion { get; set; }
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion1 { get; set; }
    public virtual ICollection<PSV_TipoTerminacion> PSV_TipoTerminacion2 { get; set; }
}
