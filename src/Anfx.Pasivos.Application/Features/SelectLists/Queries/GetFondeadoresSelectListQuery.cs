namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetFondeadoresSelectListQuery : SelectListQueryBase
{

}


internal class GetFondeadoresSelectListQueryHandler : IQueryHandler<GetFondeadoresSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetFondeadoresSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetFondeadoresSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_Fondeador
            .Select(f => new SelectItemDto
            {
                Value = f.IdFondeador,
                Text = f.Fondeador
            }).ToListAsync();

        return Result.Success(items);

    }
}
