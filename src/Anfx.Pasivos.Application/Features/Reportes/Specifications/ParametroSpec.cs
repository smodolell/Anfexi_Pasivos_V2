namespace Anfx.Pasivos.Application.Features.Reportes.Specifications;

public sealed class ParametroByFilterSpec : Specification<RSP_Parametro>
{
    public ParametroByFilterSpec(int? reporteId, string? searchText)
    {
        // Filtro por ReporteId (solo si tiene valor y no es 0)
        if (reporteId.HasValue && reporteId.Value != 0)
        {
            Query.Where(p => p.ReporteId == reporteId.Value);
        }

        // Filtro por texto de búsqueda (si no está vacío)
        if (!string.IsNullOrEmpty(searchText))
        {
            Query.Where(p => p.NomParametro.Contains(searchText));
        }
    }
}
