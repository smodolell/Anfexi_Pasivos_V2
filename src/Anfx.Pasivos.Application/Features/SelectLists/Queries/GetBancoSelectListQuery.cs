namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetBancoSelectListQuery : SelectListQueryBase
{

}



internal class GetBancoSelectListQueryHandler : IQueryHandler<GetBancoSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetBancoSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetBancoSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_Banco
            .Select(f => new SelectItemDto
            {

                Value = f.IdBanco,
                Text = f.Banco
            }).ToListAsync();

        return Result.Success(items);

    }
}