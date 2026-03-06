namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_TipoCapitalizacion
{
    public PSV_TipoCapitalizacion()
    {
        this.PSV_Contrato = new HashSet<PSV_Contrato>();
        this.Contrato = new HashSet<Contrato>();
        this.PSV_TipoTablaAmortizaTipoCapitalizacion = new HashSet<PSV_TipoTablaAmortizaTipoCapitalizacion>();
    }

    public int IdTipoCapitalizacion { get; set; }
    public string TipoCapitalizacion { get; set; }

    public virtual ICollection<PSV_Contrato> PSV_Contrato { get; set; }
    public virtual ICollection<Contrato> Contrato { get; set; }
    public virtual ICollection<PSV_TipoTablaAmortizaTipoCapitalizacion> PSV_TipoTablaAmortizaTipoCapitalizacion { get; set; }
}
