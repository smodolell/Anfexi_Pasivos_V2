using Anfx.Pasivos.Application.Features.TipoDirecciones.DTOs;

namespace Anfx.Pasivos.Application.Features.TipoDirecciones.Queries;

public class GetTiposDireccionesForExportQueryHandler : IQueryHandler<GetTiposDireccionesForExportQuery, IEnumerable<TipoDireccionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTiposDireccionesForExportQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    public async Task<IEnumerable<TipoDireccionDto>> HandleAsync(GetTiposDireccionesForExportQuery request, CancellationToken cancellationToken = default)
    {
        var query = _context.TiposDirecciones.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x => x.sTipoDireccion.ToLower().Contains(searchTerm));
        }

        var tiposDirecciones = await query
            .OrderBy(x => x.sTipoDireccion)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<TipoDireccionDto>>(tiposDirecciones);
    }
}