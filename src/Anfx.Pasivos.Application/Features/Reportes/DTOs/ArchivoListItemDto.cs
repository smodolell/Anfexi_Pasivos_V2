namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ArchivoListItemDto
{
    public Guid Id { get; set; }
    public int ReporteId { get; set; }
    public string NomReporte { get; set; } = "";
    public string NombreArchivo { get; set; } = "";
    public string ContentType { get; set; } = "";
    public string Extension { get; set; } = "";
    public DateTime FechaCreacion { get; set; }
    public string LogParameters { get; set; } = "";
}
