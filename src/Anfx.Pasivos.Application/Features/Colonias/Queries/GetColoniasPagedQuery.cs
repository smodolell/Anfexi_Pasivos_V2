using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetColoniasPagedQuery : IQuery<Result< PagedResultDto<ColoniaDto>>>
{
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
}
