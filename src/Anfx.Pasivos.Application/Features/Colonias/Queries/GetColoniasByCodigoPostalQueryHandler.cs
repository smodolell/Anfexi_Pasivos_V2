using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetColoniasByCodigoPostalQueryHandler : IQueryHandler<GetColoniasByCodigoPostalQuery, Result<ColoniaComponentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetColoniasByCodigoPostalQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result<ColoniaComponentDto>> HandleAsync(GetColoniasByCodigoPostalQuery request, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrEmpty(request.CodigoPostal))
        {
            return Result.Invalid(new ValidationError("El codigo postal Vacio"));
        }

        var colonia = await _context.Colonias
            .FirstOrDefaultAsync(c => c.CodigoPostal == request.CodigoPostal, cancellationToken);

        if (colonia == null)
            return Result.NotFound("Colonia no encontrada");

        var colonias = await _context.Colonias
            .Where(c => c.CodigoPostal == request.CodigoPostal)
            .Select(c => new SelectItemDto
            {
                Value = c.Id,
                Text = c.sColonia
            })
            .ToListAsync(cancellationToken);

        var result= new ColoniaComponentDto
        {
            Estado = colonia.Estado,
            Municipio = colonia.Municipio,
            CodigoPostal = colonia.CodigoPostal,
            Colonias = colonias
        };

        return Result.Success(result);
    }
}
