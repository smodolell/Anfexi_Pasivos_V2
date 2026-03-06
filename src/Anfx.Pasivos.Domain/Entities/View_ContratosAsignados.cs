namespace Anfx.Pasivos.Domain.Entities;

public partial class View_ContratosAsignados
{
    public int IdContrato { get; set; }
    public string Contrato { get; set; }
    public Nullable<decimal> Capital { get; set; }
    public Nullable<System.DateTime> FecActivacion { get; set; }
    public string TipoCredito { get; set; }
    public System.DateTime FechaAsignacion { get; set; }
    public int IdContratoPasivo { get; set; }
}
