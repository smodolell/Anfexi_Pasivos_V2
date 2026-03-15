using Anfx.Pasivos.Application.Features.Usuarios.DTOs;

namespace Anfx.Pasivos.Application.Features.Usuarios.Commands;

public record CreateUsuarioCommand(UsuarioCreateDto Usuario) : ICommand<Result<UsuarioDto>>;

public class CreateUsuarioCommandHandler : ICommandHandler<CreateUsuarioCommand, Result<UsuarioDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateUsuarioCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Result<UsuarioDto>> HandleAsync(CreateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            
            // Verificar si el email ya existe
            var emailExists = await _context.Usuarios
                .AnyAsync(u => u.Email == request.Usuario.Email, cancellationToken);

            if (emailExists)
            {
                return Result.Invalid(new ValidationError("El email ya está registrado"));
            }

            // Verificar si el nombre de usuario ya existe
            var usernameExists = await _context.Usuarios
                .AnyAsync(u => u.UsuarioNombre == request.Usuario.UsuarioNombre, cancellationToken);

            if (usernameExists)
            {
                return Result.Invalid(new ValidationError("El nombre de usuario ya está registrado"));
            }

            // Verificar que el rol existe
            var rolExists = await _context.Roles
                .AnyAsync(r => r.IdRol == request.Usuario.RolId, cancellationToken);

            if (!rolExists)
            {
                return Result.Invalid(new ValidationError("El rol especificado no existe"));
            }

            var usuario = _mapper.Map<Usuario>(request.Usuario);

            // Hash de la contraseña (aquí deberías usar BCrypt o similar)
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Usuario.Contrasena);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Result.Success(usuarioDto, "Usuario creado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al crear el usuario: {ex.Message}");
        }
    }
}
