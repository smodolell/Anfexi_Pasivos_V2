namespace Anfx.Pasivos.Application.Features.SelectLists.Queries;

public class GetLineaCreditoByFondeadorSelectListQuery : SelectListQueryBase
{
    public int IdFondeador { get; set; }
}


internal class GetLineaCreditoByFondeadorSelectListQueryHandler : IQueryHandler<GetLineaCreditoByFondeadorSelectListQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetLineaCreditoByFondeadorSelectListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetLineaCreditoByFondeadorSelectListQuery message, CancellationToken cancellationToken = default)
    {
        var items = await _context.PSV_LineaCredito
            .Where(r => r.IdFondeador == message.IdFondeador)
            .Select(f => new SelectItemDto
            {
                Value = f.IdLineaCredito,
                Text = string.Format("ID [{0}] -> $ {1:N2}, Disponible: $ {2:N2}", f.IdLineaCredito, f.MontoAprobado, f.MontoDisponible),
            }).ToListAsync();

        return Result.Success(items);

    }


}
