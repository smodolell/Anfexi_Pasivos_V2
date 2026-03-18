namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTipoTerminacionesSelectListQuery : SelectListQueryBase
{
}


internal class GetTipoTerminacionesSelectListQueryHandlera : IQueryHandler<GetMonedasSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTipoTerminacionesSelectListQueryHandlera(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetMonedasSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_TipoTerminacion
            .Select(f => new SelectItemDto
            {
                Value = f.IdTipoTerminacion,
                Text = f.TipoTerminacion
            }).ToListAsync();

        return Result.Success(items);

    }
}
