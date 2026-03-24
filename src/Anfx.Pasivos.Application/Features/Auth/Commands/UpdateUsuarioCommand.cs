using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public record UpdateUsuarioCommand(UsuarioUpdateDto Usuario) : ICommand<Result<UsuarioDto>>;

public class UpdateUsuarioCommandHandler : ICommandHandler<UpdateUsuarioCommand, Result<UsuarioDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<UsuarioUpdateDto> _validator;

    public UpdateUsuarioCommandHandler(IApplicationDbContext context, IMapper mapper,IValidator<UsuarioUpdateDto> validator )
    {
        _context = context;
        _mapper = mapper;
        this._validator = validator;
    }

    public async Task<Result<UsuarioDto>> HandleAsync(UpdateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request.Usuario, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == request.Usuario.Id, cancellationToken);

            if (usuario == null)
            {
                return Result.NotFound("Usuario no encontrado");
            }

            // Verificar si el email ya existe en otro usuario
            var emailExists = await _context.Usuarios
                .AnyAsync(u => u.Email == request.Usuario.Email && u.IdUsuario != request.Usuario.Id, cancellationToken);

            if (emailExists)
            {
                return Result.Invalid(new ValidationError("El email ya está registrado en otro usuario"));
            }

            // Verificar si el nombre de usuario ya existe en otro usuario
            var usernameExists = await _context.Usuarios
                .AnyAsync(u => u.UserName == request.Usuario.UsuarioNombre && u.IdUsuario != request.Usuario.Id, cancellationToken);

            if (usernameExists)
            {
                return Result.Invalid(new ValidationError("El nombre de usuario ya está registrado en otro usuario"));
            }

            // Verificar que el rol existe
            var rolExists = await _context.Roles
                .AnyAsync(r => r.IdRol == request.Usuario.RolId, cancellationToken);

            if (!rolExists)
            {
                return Result.Invalid(new ValidationError("El rol especificado no existe"));
            }

            // Actualizar propiedades
            usuario.NombreCompleto = request.Usuario.NombreCompleto;
            usuario.Email = request.Usuario.Email;
            usuario.UserName = request.Usuario.UsuarioNombre;
            usuario.IdRol = request.Usuario.RolId;
            
            // Actualizar contraseña si se proporciona
            if (!string.IsNullOrEmpty(request.Usuario.Contrasena))
            {
                usuario.UserPass = BCrypt.Net.BCrypt.HashPassword(request.Usuario.Contrasena);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Result.Success(usuarioDto, "Usuario actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al actualizar el usuario: {ex.Message}");
        }
    }
}
