namespace Anfx.Pasivos.Domain.Entities;

public partial class PSV_RelActivoPasivo
{
    public int IdContratoActivo { get; set; }
    public int IdContratoPasivo { get; set; }
    public System.DateTime FechaAsignacion { get; set; }
    public int IdUsuario_Asigno { get; set; }

    public virtual Contrato Contrato { get; set; }
    public virtual PSV_Contrato PSV_Contrato { get; set; }
}
