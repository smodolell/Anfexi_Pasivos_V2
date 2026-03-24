namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetTipoPagoSelectListQuery : SelectListQueryBase
{

}



internal class GetTipoPagoSelectListQueryHandler : IQueryHandler<GetTipoPagoSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTipoPagoSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetTipoPagoSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_TipoPago
            .Select(f => new SelectItemDto
            {
                Value = f.IdTipoPago,
                Text = f.TipoPago
            }).ToListAsync();

        return Result.Success(items);

    }
}