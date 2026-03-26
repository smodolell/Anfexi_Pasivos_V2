namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTipoReduccionSelectListQuery : SelectListQueryBase
{
}


internal class GetTipoReduccionSelectListQueryHandler : IQueryHandler<GetTipoReduccionSelectListQuery, Result<List<SelectItemDto>>>
{
    
    
    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetTipoReduccionSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = new List<SelectItemDto>
        {
            new SelectItemDto { Text = "POR MONTO", Value = 1 },
            //new SelectItemDto { Text = "POR PLAZO", Value = 2 }
        };

        return await Task.FromResult(Result.Success(items));;

    }
}
