namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetContratoActivoByPasivoSelectListQuery : SelectListQueryBase
{
    public int IdContratoPasivo { get; set; }
}


internal class GetContratoActivoByPasivoSelectListQueryHandler : IQueryHandler<GetContratoActivoByPasivoSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetContratoActivoByPasivoSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetContratoActivoByPasivoSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_RelActivoPasivo
            .Include(i => i.Contrato)
            .Where(c => c.IdContratoPasivo == message.IdContratoPasivo)
            .Select(f => new SelectItemDto
            {
                Value = f.Contrato.IdContrato,
                Text = f.Contrato.Contrato1
            }).ToListAsync();

        return Result.Success(items);
    }
}