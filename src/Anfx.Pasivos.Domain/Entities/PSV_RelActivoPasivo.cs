namespace Anfx.Pasivos.Domain.Entities;

public class PSV_RelActivoPasivo
{
    public int IdContratoActivo { get; set; }
    public int IdContratoPasivo { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public int IdUsuario_Asigno { get; set; }

    public virtual Contrato Contrato { get; set; } =null!;
    public virtual PSV_Contrato PSV_Contrato { get; set; } = null!;
}
