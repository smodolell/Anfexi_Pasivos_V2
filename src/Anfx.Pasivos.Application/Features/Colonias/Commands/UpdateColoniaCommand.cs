using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Commands;

public record UpdateColoniaCommand : ICommand<Result< ColoniaDto>>
{
    public int Id { get; init; }
    public string sColonia { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string Municipio { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
}
