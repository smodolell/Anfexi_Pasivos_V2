namespace Anfx.Pasivos.Application.Features.Reportes.Specifications;

public sealed class ArchivoByReporteIdSpec : Specification<RSP_Archivo>
{
    public ArchivoByReporteIdSpec(int? reporteId)
    {
        // Solo aplicar el filtro si reporteId tiene valor y no es 0
        if (reporteId.HasValue && reporteId.Value != 0)
        {
            Query.Where(archivo => archivo.ReporteId == reporteId.Value);
        }
    }
}
