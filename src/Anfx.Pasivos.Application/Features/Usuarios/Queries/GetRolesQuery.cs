namespace Anfx.Pasivos.Application.Features.Usuarios.Queries;

public record GetRolesQuery : IQuery<Result<IEnumerable<SelectItemDto>>>;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, Result<IEnumerable<SelectItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRolesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<SelectItemDto>>> HandleAsync(GetRolesQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _context.Roles
                .Where(r => r.Activo)
                .OrderBy(r => r.sRol)
                .ToListAsync(cancellationToken);

            var rolesDto = _mapper.Map<IEnumerable<SelectItemDto>>(roles);
            return Result.Success(rolesDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener los roles: {ex.Message}");
        }
    }
}
