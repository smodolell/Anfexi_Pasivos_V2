using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Queries;

public record GetRolByIdQuery(int Id) : IQuery<Result<RolDto>>;

public class GetRolByIdQueryHandler : IQueryHandler<GetRolByIdQuery, Result<RolDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRolByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<RolDto>> HandleAsync(GetRolByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.IdRol == request.Id /*&& r.Activo*/, cancellationToken);

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
