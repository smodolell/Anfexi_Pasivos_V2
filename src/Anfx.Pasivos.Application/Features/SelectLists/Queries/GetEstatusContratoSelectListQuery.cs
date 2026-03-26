namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetEstatusContratoSelectListQuery : SelectListQueryBase
{
}


internal class GetEstatusContratoSelectListQueryHandlera : IQueryHandler<GetEstatusContratoSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetEstatusContratoSelectListQueryHandlera(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetEstatusContratoSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_EstatusContrato
            .Select(f => new SelectItemDto
            {
                Value = f.IdEstatusContrato,
                Text = f.EstatusContrato
            }).ToListAsync();

        return Result.Success(items);

    }
}
