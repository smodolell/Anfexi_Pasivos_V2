using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public record GetTipoDireccionByIdQuery : IQuery<Result<TipoDireccionDto>>
{
    public int Id { get; init; }
}
