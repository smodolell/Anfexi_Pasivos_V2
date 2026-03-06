using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetColoniasByIdQueryHandler : IQueryHandler<GetColoniasByIdQuery,Result< ColoniaComponentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetColoniasByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ColoniaComponentDto>> HandleAsync(GetColoniasByIdQuery request, CancellationToken cancellationToken = default)
    {
        var colonia = await _context.Colonias
            .SingleOrDefaultAsync(c => c.Id == request.Id,cancellationToken);

        if (colonia == null)
            return Result.NotFound("Colonia no Encontrada");

        var colonias = await _context.Colonias
            .Where(c => c.CodigoPostal == colonia.CodigoPostal)
            .Select(c => new SelectItemDto
            {
                Value = c.Id,
                Text = c.sColonia
            })
            .ToListAsync(cancellationToken);

        var result = new ColoniaComponentDto
        {
            Estado = colonia.Estado,
            Municipio = colonia.Municipio,
            CodigoPostal = colonia.CodigoPostal,
            Colonias = colonias
        };

        return result;
    }
}
