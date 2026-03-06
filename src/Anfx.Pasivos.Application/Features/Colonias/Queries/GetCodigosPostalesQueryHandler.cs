namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetCodigosPostalesQueryHandler : IQueryHandler<GetCodigosPostalesQuery, Result<List<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCodigosPostalesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SelectItemDto>>> HandleAsync(GetCodigosPostalesQuery request, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrEmpty(request.CodigoPostal))
        {
            return Result.Invalid(new ValidationError("El codigo postal Vacio"));
        }
        var colonias = await _context.Colonias
            .Where(c => c.CodigoPostal.Contains(request.CodigoPostal))
            .Select(c => new SelectItemDto
            {
                Value = c.Id,
                Text = c.sColonia
            })
            .ToListAsync(cancellationToken);

        return Result.Success(colonias);
    }
}
