using Anfx.Pasivos.Application.Common;

namespace Anfx.Pasivos.Application.Features.Reportes.DTOs;

public class ReporteExportDto
{
    public byte[] Data { get; set; } = [];
    public string ContentType { get; set; } = ApplicationConstants.ContentType_Excel;
    public string FileName { get; set; } = "";
}
