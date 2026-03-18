namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTipoMovimientoSelectListQuery : SelectListQueryBase
{

}



internal class GetTipoMovimientoSelectListQueryHandler : IQueryHandler<GetTipoMovimientoSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTipoMovimientoSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetTipoMovimientoSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.TipoMovimiento
            .Select(f => new SelectItemDto
            {
                Value = f.IdTipoMovimiento,
                Text = f.TipoMovimiento1
            }).ToListAsync();

        return Result.Success(items);

    }
}