namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public record GetCodigosPostalesQuery : IQuery<Result<List<SelectItemDto>>>
{
    public string CodigoPostal { get; init; } = string.Empty;
}
