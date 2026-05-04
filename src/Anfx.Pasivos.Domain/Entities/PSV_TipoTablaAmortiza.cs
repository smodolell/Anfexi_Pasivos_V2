namespace Anfx.Pasivos.Domain.Entities;

public class PSV_TipoTablaAmortiza
{
  
    public int IdTipoTablaAmortiza { get; set; }
    public string TipoTablaAmortiza { get; set; } = string.Empty;
    public bool EsCapitalizable { get; set; }
    public bool Activo { get; set; }

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; } = new HashSet<PSV_Contrato>();
    public virtual ICollection<PSV_TipoCredito> PSV_TipoCredito { get; set; } = new HashSet<PSV_TipoCredito>();
    public virtual ICollection<PSV_TipoTablaAmortizaPeriodicidad> PSV_TipoTablaAmortizaPeriodicidad { get; set; } = new HashSet<PSV_TipoTablaAmortizaPeriodicidad>();
    public virtual ICollection<Contrato> Contrato { get; set; } = new HashSet<Contrato>();
    public virtual ICollection<PSV_TipoTablaAmortizaTipoCapitalizacion> PSV_TipoTablaAmortizaTipoCapitalizacion { get; set; } = new HashSet<PSV_TipoTablaAmortizaTipoCapitalizacion>();
    public virtual ICollection<PSV_TipoTablaAmortizaTipoPagoCapital> PSV_TipoTablaAmortizaTipoPagoCapital { get; set; } = new HashSet<PSV_TipoTablaAmortizaTipoPagoCapital>();
}
