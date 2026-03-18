namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetContratoPasivoByIdFondeadorSelectListQuery : SelectListQueryBase
{
    public int IdFondeador { get; set; }

    public bool? EstatusActivo { get; set; }
}


public class GetContratoPasivoByIdFondeadorSelectListQueryHandler : IQueryHandler<GetContratoPasivoByIdFondeadorSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetContratoPasivoByIdFondeadorSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetContratoPasivoByIdFondeadorSelectListQuery message, CancellationToken cancellationToken = default)
    {

        var items = _context.PSV_Contrato
            .Where(c => c.IdFondeador == message.IdFondeador);

        if (message.EstatusActivo.HasValue)
        {
            if (message.EstatusActivo.Value)
            {
                items = items.Where(r => r.IdEstatusContrato == 2);
            }
            else
            {
                items = items.Where(r => r.IdEstatusContrato != 2);
            }
        }
        var result = await items.Select(f => new SelectItemDto
        {
            Value = f.IdContrato,
            Text = f.Contrato
        }).ToListAsync();

        return Result.Success(result);


    }
}
