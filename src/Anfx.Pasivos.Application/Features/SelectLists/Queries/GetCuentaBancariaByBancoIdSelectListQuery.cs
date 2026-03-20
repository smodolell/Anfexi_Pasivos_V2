using Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetCuentaBancariaByBancoIdSelectListQuery : SelectListQueryBase
{
    public int IdBanco { get; set; }
}



internal class GetCuentaBancariaByBancoIdSelectListQueryHandler : IQueryHandler<GetCuentaBancariaByBancoIdSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetCuentaBancariaByBancoIdSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetCuentaBancariaByBancoIdSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_CuentaBancaria
            .Where(r => r.IdBanco == message.IdBanco)
            .Select(f => new SelectItemDto
            {
                Value = f.IdCuentaBancaria,
                Text = f.CuentaBancaria
            }).ToListAsync();

        return Result.Success(items);

    }
}