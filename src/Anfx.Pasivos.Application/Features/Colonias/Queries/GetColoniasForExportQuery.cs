using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetColoniasForExportQuery : IQuery<Result<IEnumerable<ColoniaDto>>>
{
    public string? SearchTerm { get; init; }
}
