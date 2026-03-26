namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetPeriodicidadSelectListQuery : SelectListQueryBase
{
}


internal class GetPeriodicidadSelectListQueryHandlera : IQueryHandler<GetPeriodicidadSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetPeriodicidadSelectListQueryHandlera(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetPeriodicidadSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.SB_Periodicidad
            .Where(w => w.Activo == true)
            .Select(f => new SelectItemDto
            {
                Value = f.IdPeriodicidad,
                Text = f.DescPeriodicidad
            }).ToListAsync();

        return Result.Success(items);

    }
}
