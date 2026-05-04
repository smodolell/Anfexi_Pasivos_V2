namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ReporteListItemDto
{
    public int Id { get; set; }
    public string NomReporte { get; set; } = "";
    public string StoredProcedure { get; set; } = "";
    public string Parametros { get; set; } = "";
    public bool Activo { get; set; }
}
