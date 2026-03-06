using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public record GetTipoDireccionesQuery : IQuery<Result<IEnumerable<TipoDireccionDto>>>
{
}
