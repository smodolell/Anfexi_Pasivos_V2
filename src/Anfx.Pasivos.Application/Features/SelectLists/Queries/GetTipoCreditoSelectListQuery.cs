namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTipoCreditoSelectListQuery : SelectListQueryBase
{
}


internal class GetTipoCreditoSelectListQueryHandlera : IQueryHandler<GetTipoCreditoSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTipoCreditoSelectListQueryHandlera(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetTipoCreditoSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_TipoCredito
            .Select(f => new SelectItemDto
            {
                Value = f.IdTipoCredito,
                Text = f.TipoCredito
            }).ToListAsync();

        return Result.Success(items);

    }
}
