using Anfx.Pasivos.Application.Features.Colonias.DTOs;

namespace Anfx.Pasivos.Application.Features.Colonias.Queries;

public class GetColoniaByIdQueryHandler : IQueryHandler<GetColoniaByIdQuery, Result<ColoniaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetColoniaByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Result<ColoniaDto>> HandleAsync(GetColoniaByIdQuery request, CancellationToken cancellationToken = default)
    {
       


        var colonia = await _context.Colonias
            .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (colonia == null)
        {
            return Result.NotFound("Colonia no encontrada");
        }

        var result = _mapper.Map<ColoniaDto>(colonia);

        return Result.Success(result);
    }
}
