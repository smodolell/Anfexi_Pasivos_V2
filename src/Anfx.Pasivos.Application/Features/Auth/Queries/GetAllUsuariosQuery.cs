using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public class GetAllUsuariosQuery : IQuery<Result<List<UsuarioDto>>>
{
}

public class GetAllUsuariosQueryHandler : IQueryHandler<GetAllUsuariosQuery, Result<List<UsuarioDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllUsuariosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<UsuarioDto>>> HandleAsync(GetAllUsuariosQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync(cancellationToken);

            var usuariosDto = _mapper.Map<List<UsuarioDto>>(usuarios);
            return Result.Success(usuariosDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error interno del servidor {ex.Message}");
        }
    }
}
