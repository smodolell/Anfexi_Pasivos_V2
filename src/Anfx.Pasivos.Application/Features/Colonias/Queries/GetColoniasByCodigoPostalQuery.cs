using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetColoniasByCodigoPostalQuery : IQuery<Result<ColoniaComponentDto>>
{
    public string CodigoPostal { get; init; } = string.Empty;
}
