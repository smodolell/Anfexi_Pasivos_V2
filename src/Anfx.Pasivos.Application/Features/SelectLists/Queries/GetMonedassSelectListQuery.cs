namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetMonedassSelectListQuery : SelectListQueryBase
{

}

internal class GetMonedassSelectListQueryHandler : IQueryHandler<GetFondeadoresSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetMonedassSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetFondeadoresSelectListQuery message, CancellationToken cancellationToken = default)
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
