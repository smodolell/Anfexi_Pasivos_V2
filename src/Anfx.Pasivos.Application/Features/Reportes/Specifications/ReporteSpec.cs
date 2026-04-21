namespace Anfx.Pasivos.Application.Features.Reportes.Specifications;

public sealed class ReporteBySearchTextSpec : Specification<RSP_Reporte>
{
    public ReporteBySearchTextSpec(string? searchText)
    {
        if (!string.IsNullOrEmpty(searchText))
        {
            Query.Where(p => p.NomReporte.Contains(searchText) ||
                            p.StoredProcedure.Contains(searchText));
        }
    }
}