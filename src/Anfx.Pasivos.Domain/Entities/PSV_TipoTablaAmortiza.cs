namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TipoTablaAmortiza
{
    public PSV_TipoTablaAmortiza()
    {
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
        this.PSV_TipoCredito = new HashSet<PSV_TipoCredito>();
        this.PSV_TipoTablaAmortizaPeriodicidad = new HashSet<PSV_TipoTablaAmortizaPeriodicidad>();
        this.Contrato = new HashSet<Contrato>();
        this.PSV_TipoTablaAmortizaTipoCapitalizacion = new HashSet<PSV_TipoTablaAmortizaTipoCapitalizacion>();
        this.PSV_TipoTablaAmortizaTipoPagoCapital = new HashSet<PSV_TipoTablaAmortizaTipoPagoCapital>();
    }

    public int IdTipoTablaAmortiza { get; set; }
    public string TipoTablaAmortiza { get; set; }
    public bool EsCapitalizable { get; set; }
    public bool Activo { get; set; }

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito { get; set; }
    public virtual ICollection<PSV_TipoTablaAmortizaPeriodicidad> PSV_TipoTablaAmortizaPeriodicidad { get; set; }
    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<PSV_TipoTablaAmortizaTipoCapitalizacion> PSV_TipoTablaAmortizaTipoCapitalizacion { get; set; }
    public virtual ICollection<PSV_TipoTablaAmortizaTipoPagoCapital> PSV_TipoTablaAmortizaTipoPagoCapital { get; set; }
}
