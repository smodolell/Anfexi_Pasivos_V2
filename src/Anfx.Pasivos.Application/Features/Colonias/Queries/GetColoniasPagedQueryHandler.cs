using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetColoniasPagedQueryHandler : IQueryHandler<GetColoniasPagedQuery,Result< PagedResultDto<ColoniaDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetColoniasPagedQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Result<PagedResultDto<ColoniaDto>>> HandleAsync(GetColoniasPagedQuery request, CancellationToken cancellationToken = default)
    {
        var query = _context.Colonias.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => c.sColonia.Contains(request.SearchTerm) ||
                                    c.Estado.Contains(request.SearchTerm) ||
                                    c.Municipio.Contains(request.SearchTerm) ||
                                    c.CodigoPostal.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (request.SortBy?.ToLower(), request.SortDir?.ToLower() == "desc") switch
        {
            ("id",          true)  => query.OrderByDescending(c => c.Id),
            ("id",          false) => query.OrderBy(c => c.Id),
            ("scolonia",    true)  => query.OrderByDescending(c => c.sColonia),
            ("scolonia",    false) => query.OrderBy(c => c.sColonia),
            ("estado",      true)  => query.OrderByDescending(c => c.Estado),
            ("estado",      false) => query.OrderBy(c => c.Estado),
            ("municipio",   true)  => query.OrderByDescending(c => c.Municipio),
            ("municipio",   false) => query.OrderBy(c => c.Municipio),
            _                      => query.OrderBy(c => c.Id),
        };

        var colonias = await query
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToListAsync(cancellationToken);

        var coloniasDto = _mapper.Map<IEnumerable<ColoniaDto>>(colonias);

        var result = new PagedResultDto<ColoniaDto>
        {
            Results = coloniasDto,
            TotalCount = totalCount,
            CurrentPage = request.Page,
            PageSize = request.Size,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.Size)
        };

        return Result.Success(result);
    }
}
