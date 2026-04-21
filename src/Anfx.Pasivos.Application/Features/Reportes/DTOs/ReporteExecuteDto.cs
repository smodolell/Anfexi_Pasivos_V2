namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ReporteExecuteDto
{
    public int? ReporteId { get; set; }
    public int ReporteFormatoId { get; set; }
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public List<ReporteExecuteParametroDto> Parametros { get; set; } = [];
    public string NomReporte { get; set; } = "";
    public string Titulo { get; set; } = "";
    public string Data_FechaActual { get; set; } = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
    public string Data_FechaInicioMes { get; set; } = "";
    public string Data_FechaFinMes { get; set; } = "";
}
