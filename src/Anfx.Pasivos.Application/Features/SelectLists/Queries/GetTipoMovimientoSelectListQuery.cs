using Anfx.Pasivos.Application.Features.SelectLists.Specifications;

namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTipoMovimientoSelectListQuery : SelectListQueryBase
{
    public bool? Capturable { get; set; }
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
        var spec = new TipoMovimientoSpec(message.Capturable);

        var query = SpecificationEvaluator.Default.GetQuery(
            _context.TipoMovimiento,
            spec
        );
        var items = await query 
            .Select(f => new SelectItemDto
            {
                Value = f.IdTipoMovimiento,
                Text = f.TipoMovimiento1
            }).ToListAsync();

        return Result.Success(items);

    }
}