namespace Anfx.Pasivos.Application.Features.Empresas.Queries;

public record GetAllTiposDireccionesQuery : IQuery<Result<IEnumerable<SelectItemDto>>>;

public class GetAllTiposDireccionesQueryHandler : IQueryHandler<GetAllTiposDireccionesQuery, Result<IEnumerable<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllTiposDireccionesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<SelectItemDto>>> HandleAsync(GetAllTiposDireccionesQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var tiposdirecciones = await _context.TiposDirecciones
                .OrderBy(e => e.sTipoDireccion)
                .ToListAsync(cancellationToken);

            var empresasDto = _mapper.Map<IEnumerable<SelectItemDto>>(tiposdirecciones);
            return Result.Success(empresasDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener las empresas: {ex.Message}");
        }
    }
}
