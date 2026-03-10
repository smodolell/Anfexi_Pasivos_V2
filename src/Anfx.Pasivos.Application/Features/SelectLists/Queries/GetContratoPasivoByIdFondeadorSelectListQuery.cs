namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetContratoPasivoByIdFondeadorSelectListQuery : SelectListQueryBase
{
    public int IdFondeador { get; set; }
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

        var items = await _context.PSV_Contrato
            .Where(c => c.IdFondeador == message.IdFondeador)
           .Select(f => new SelectItemDto
           {
               Value = f.IdContrato,
               Text = f.Contrato
           }).ToListAsync();

        return Result.Success(items);


    }
}
                       