using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<UsuarioLoginDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IApplicationDbContext context, IMapper mapper, IJwtService jwtService)
    {
        _context = context;
        _mapper = mapper;
        _jwtService = jwtService;
    }


    public async Task<Result<UsuarioLoginDto>> HandleAsync(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u =>
            (!string.IsNullOrEmpty(request.Email) && u.Email == request.Email) ||
        (!string.IsNullOrEmpty(request.Usuario) && u.UsuarioNombre == request.Usuario),

        cancellationToken);

        if (usuario == null || !usuario.Activo)
        {
            return Result.Unauthorized("Credenciales inválidas");
        }


        bool contrasenaValida = BCrypt.Net.BCrypt.Verify(
           request.Contrasenia,
           usuario.Contrasena
       );
        if (!contrasenaValida)
        {
            return Result.Unauthorized("Credenciales inválidas");
        }

        var loginResponse = new UsuarioLoginDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            UsuarioNombre = usuario.UsuarioNombre,
            Role = usuario.Rol.sRol,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            TokenExpiration = DateTime.UtcNow.AddMinutes(60)
        };

        // Generar JWT token usando el servicio real
        loginResponse.Token = _jwtService.GenerateToken(loginResponse);

        return Result.Success(loginResponse, "Login exitoso");
    }
}
