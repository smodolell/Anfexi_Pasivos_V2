using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetColoniasByIdQuery : IQuery<Result<ColoniaComponentDto>>
{
    public int Id { get; init; }
}
