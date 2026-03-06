using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetColoniasQueryHandler : IQueryHandler<GetColoniasQuery, Result<IEnumerable<ColoniaDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetColoniasQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ColoniaDto>>> HandleAsync(GetColoniasQuery message, CancellationToken cancellationToken = default)
    {
        var colonias = await _context.Colonias
          .ToListAsync(cancellationToken);

        var result = _mapper.Map<IEnumerable<ColoniaDto>>(colonias);


        return Result.Success(result);
    }
}
