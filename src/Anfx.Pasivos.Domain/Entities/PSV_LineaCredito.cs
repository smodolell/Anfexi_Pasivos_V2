namespace Anfx.Pasivos.Domain.Entities;

public class PSV_LineaCredito
{
    public int IdLineaCredito { get; set; }
    public int IdFondeador { get; set; }
    public int IdMoneda { get; set; }
    public decimal MontoAprobado { get; set; }
    public decimal MontoDispuesto { get; set; }
    public decimal MontoDisponible { get; set; }
    public decimal MontoRevolvente { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaUltimaDisposicion { get; set; }
    public DateTime? FechaMaxDisposicion { get; set; }
    public DateTime? FechaAmpliacion { get; set; }
    public int NoDisposiciones { get; set; }
    public int PlazoMaximo { get; set; }
    public bool EsRevolvente { get; set; }
    public bool Activo { get; set; }
    public int? IdTasa { get; set; }
    public decimal? Tasa { get; set; }

    public virtual PSV_Fondeador PSV_Fondeador { get; set; } = null!;
    public virtual SB_TipoMoneda SB_TipoMoneda { get; set; } = null!;
    public virtual Tasa? Tasa1 { get; set; }
    public virtual ICollection<PSV_RelLineaCreditoTipoCredito> PSV_RelLineaCreditoTipoCredito { get; set; } = new HashSet<PSV_RelLineaCreditoTipoCredito>();
    public virtual ICollection<PSV_RelLineaCreditoContrato> PSV_RelLineaCreditoContrato { get; set; } = new HashSet<PSV_RelLineaCreditoContrato>();
}
