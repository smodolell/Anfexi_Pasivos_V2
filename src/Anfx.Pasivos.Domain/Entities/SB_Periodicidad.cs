namespace Anfx.Pasivos.Domain.Entities;

public class SB_Periodicidad
{
    public SB_Periodicidad()
    {
        this.Contrato = new HashSet<Contrato>();
        this.PSV_TipoTablaAmortizaPeriodicidad = new HashSet<PSV_TipoTablaAmortizaPeriodicidad>();
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
        this.PSV_Contrato1 = new HashSet<PSV_Contrato>();
        this.Contrato1 = new HashSet<Contrato>();
        this.Contrato2 = new HashSet<Contrato>();
    }

    public int IdPeriodicidad { get; set; }
    public string CveCortaPeriodicidad { get; set; }
    public string DescPeriodicidad { get; set; }
    public int? ParamDias { get; set; }
    public int? ParamMes { get; set; }
    public bool sDefault { get; set; }
    public bool Band { get; set; }
    public bool? PedirDiasVencimiento { get; set; }
    public int? CantidadDiasVencimiento { get; set; }
    public bool? Activo { get; set; }
    public int? NoPagosMes { get; set; }
    public decimal NroPagosAnio { get; set; }

   
    public virtual ICollection<PSV_TipoTablaAmortizaPeriodicidad> PSV_TipoTablaAmortizaPeriodicidad { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<PSV_Contrato> PSV_Contrato1 { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<Contrato> Contrato1 { get; set; }
    public virtual ICollection<Contrato> Contrato2 { get; set; }
}
