using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Queries;

public record GetRolByNameQuery(string Nombre) : IQuery<Result<RolDto>>;

internal class GetRolByNameQueryHandler : IQueryHandler<GetRolByNameQuery, Result<RolDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRolByNameQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<RolDto>> HandleAsync(GetRolByNameQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.sRol == request.Nombre && r.Activo, cancellationToken);

            if (rol == null)
            {
                return Result.NotFound("Rol no encontrado");
            }

            var rolDto = _mapper.Map<RolDto>(rol);
            return Result.Success(rolDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener el rol: {ex.Message}");
        }
    }
}
