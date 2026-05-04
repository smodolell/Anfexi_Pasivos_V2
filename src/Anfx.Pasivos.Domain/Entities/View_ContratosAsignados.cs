namespace Anfx.Pasivos.Domain.Entities;

public class View_ContratosAsignados
{
    public int IdContrato { get; set; }
    public string Contrato { get; set; } = string.Empty;
    public decimal? Capital { get; set; }
    public DateTime? FecActivacion { get; set; }
    public string TipoCredito { get; set; } = string.Empty;
    public DateTime? FechaAsignacion { get; set; }
    public int IdContratoPasivo { get; set; }
}
