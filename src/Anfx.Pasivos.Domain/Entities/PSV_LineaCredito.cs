namespace Anfx.Pasivos.Domain.Entities;

public class PSV_LineaCredito
{
    public PSV_LineaCredito()
    {
        this.PSV_RelLineaCreditoTipoCredito = new HashSet<PSV_RelLineaCreditoTipoCredito>();
        //this.PSV_Contrato = new HashSet<PSV_Contrato>();
    }

    public int IdLineaCredito { get; set; }
    public int IdFondeador { get; set; }
    public int IdMoneda { get; set; }
    public decimal MontoAprobado { get; set; }
    public decimal MontoDispuesto { get; set; }
    public decimal MontoDisponible { get; set; }
    public decimal MontoRevolvente { get; set; }
    public Nullable<System.DateTime> FechaAprobacion { get; set; }
    public Nullable<System.DateTime> FechaUltimaDisposicion { get; set; }
    public Nullable<System.DateTime> FechaMaxDisposicion { get; set; }
    public Nullable<System.DateTime> FechaAmpliacion { get; set; }
    public int NoDisposiciones { get; set; }
    public int PlazoMaximo { get; set; }
    public bool EsRevolvente { get; set; }
    public bool Activo { get; set; }
    public Nullable<int> IdTasa { get; set; }
    public Nullable<decimal> Tasa { get; set; }

    public virtual PSV_Fondeador PSV_Fondeador { get; set; }
    public virtual SB_TipoMoneda SB_TipoMoneda { get; set; }
    public virtual Tasa Tasa1 { get; set; }
    public virtual ICollection<PSV_RelLineaCreditoTipoCredito> PSV_RelLineaCreditoTipoCredito { get; set; }
    public virtual ICollection<PSV_RelLineaCreditoContrato> PSV_RelLineaCreditoContrato { get; set; }
    //public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
}
