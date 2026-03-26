using Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTasaIvaSelectListQuery : SelectListQueryBase
{
}

internal class GetTasaIvaSelectListQueryHandler
    : IQueryHandler<GetTasaIvaSelectListQuery, Result<List<SelectItemDto>>>
{
    public Task<Result<List<SelectItemDto>>> HandleAsync(
        GetTasaIvaSelectListQuery message,
        CancellationToken cancellationToken = default)
    {
        var items = new List<SelectItemDto>
        {
            new SelectItemDto { ValueDecimal = 16.0000m, Text = "16 %" },
            new SelectItemDto { ValueDecimal = 11.0000m, Text = "11 %" },
            new SelectItemDto { ValueDecimal = 0.0000m, Text = "0 %" }
        };

        return Task.FromResult(Result<List<SelectItemDto>>.Success(items));
    }
}