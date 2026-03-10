namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;


public partial class CarteraMensualDto
{
    public int? Id { get; set; }
    public DateOnly? FecIni { get; set; }
    public DateOnly? FecFin { get; set; }
    public decimal Capital { get; set; }
    public decimal Interes { get; set; }
}
