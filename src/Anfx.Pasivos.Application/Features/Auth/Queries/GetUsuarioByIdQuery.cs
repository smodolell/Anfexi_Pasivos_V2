using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Queries;

public class GetUsuarioByIdQuery : IQuery<Result<UsuarioDto>>
{
    public int Id { get; set; }

    public GetUsuarioByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetUsuarioByIdQueryHandler : IQueryHandler<GetUsuarioByIdQuery, Result<UsuarioDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUsuarioByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<UsuarioDto>> HandleAsync(GetUsuarioByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == request.Id, cancellationToken);

            if (usuario == null)
            {
                return Result.NotFound("Usuario no encontrado");
            }

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Result.Success(usuarioDto);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error interno del servidor {ex.InnerException}");
        }
    }
}
