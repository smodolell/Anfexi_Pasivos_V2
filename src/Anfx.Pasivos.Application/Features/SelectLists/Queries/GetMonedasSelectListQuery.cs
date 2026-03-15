namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetMonedasSelectListQuery : SelectListQueryBase
{
}

internal class GetMonedasSelectListQueryHandler : IQueryHandler<GetMonedasSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetMonedasSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetMonedasSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.SB_TipoMoneda
            .Select(f => new SelectItemDto
            {
                Value = f.IdTipoMoneda,
                Text = f.DescTipoMoneda
            }).ToListAsync();

        return Result.Success(items);

    }
}
