using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetColoniaByIdQuery : IQuery<Result<ColoniaDto>>
{
    public int Id { get; init; }
}
