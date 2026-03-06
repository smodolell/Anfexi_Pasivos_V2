using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public class GetTiposDireccionesForExportQuery : IQuery<IEnumerable<TipoDireccionDto>>
{
    public string? SearchTerm { get; set; }
}