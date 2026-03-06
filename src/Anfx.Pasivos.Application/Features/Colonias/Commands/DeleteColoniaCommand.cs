namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public record DeleteColoniaCommand : ICommand<Result>
{
    public int Id { get; init; }
}
