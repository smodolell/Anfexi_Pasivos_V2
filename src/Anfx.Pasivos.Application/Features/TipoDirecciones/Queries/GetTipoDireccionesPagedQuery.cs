using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public record GetTipoDireccionesPagedQuery : IQuery<Result<PagedResultDto<TipoDireccionDto>>>
{
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
}
