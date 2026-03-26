namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetPeriodicidadTTASelectListQuery : SelectListQueryBase
{
    public int IdTipoTablaAmortiza { get; set; }
}


internal class GetPeriodicidadTTASelectListQueryHandlera : IQueryHandler<GetPeriodicidadTTASelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetPeriodicidadTTASelectListQueryHandlera(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetPeriodicidadTTASelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_TipoTablaAmortizaPeriodicidad
            .Include(i => i.SB_Periodicidad)
            .Where(r => r.IdTipoTablaAmortiza == message.IdTipoTablaAmortiza)
            .Select(f => new SelectItemDto
            {
                Value = f.IdPeriodicidad,
                Text = f.SB_Periodicidad.DescPeriodicidad
            }).ToListAsync();

        return Result.Success(items);

    }
}
