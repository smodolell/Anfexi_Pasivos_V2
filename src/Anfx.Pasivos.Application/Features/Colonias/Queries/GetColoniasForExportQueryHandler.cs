using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetColoniasForExportQueryHandler : IQueryHandler<GetColoniasForExportQuery, Result< IEnumerable<ColoniaDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetColoniasForExportQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result< IEnumerable<ColoniaDto>>> HandleAsync(GetColoniasForExportQuery request, CancellationToken cancellationToken = default)
    {
        var query = _context.Colonias.AsQueryable();

        // Aplicar el mismo filtro que GetColoniasPagedQuery
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => c.sColonia.Contains(request.SearchTerm) ||
                                    c.Estado.Contains(request.SearchTerm) ||
                                    c.Municipio.Contains(request.SearchTerm) ||
                                    c.CodigoPostal.Contains(request.SearchTerm));
        }

        // Obtener todos los datos sin paginación
        var colonias = await query.Take(1000).ToListAsync(cancellationToken);

        var result = _mapper.Map<IEnumerable<ColoniaDto>>(colonias);
        return Result.Success(result);


    }
}
