namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Commands;

public record DeleteTipoDireccionCommand : ICommand<Result>
{
    public int Id { get; init; }
}
