namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;


public class GetTasaSelectListQuery : SelectListQueryBase
{
    public bool EsVariable { get; set; }
}


internal class GetTasaSelectListQueryHandler : IQueryHandler<GetTasaSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTasaSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetTasaSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.Tasa
            .Where(w => w.EsVariable == message.EsVariable)
            .Select(f => new SelectItemDto
            {
                Value = f.IdTasa,
                ValueDecimal = f.Valor ?? 0,
                Text = f.Tasa1
            }).ToListAsync();

        return Result.Success(items);

    }
}
