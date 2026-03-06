using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;

public record UpdateTipoDireccionCommand : ICommand<Result<TipoDireccionDto>>
{
    public int Id { get; init; }
    public string sTipoDireccion { get; init; } = string.Empty;
}
