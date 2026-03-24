using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public record CreateUsuarioCommand(UsuarioCreateDto Usuario) : ICommand<Result<UsuarioDto>>;

public class CreateUsuarioCommandHandler : ICommandHandler<CreateUsuarioCommand, Result<UsuarioDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<UsuarioCreateDto> _validator;

    public CreateUsuarioCommandHandler(IApplicationDbContext context, IMapper mapper, IValidator<UsuarioCreateDto> validator)
    {
        _context = context;
        _mapper = mapper;
        this._validator = validator;
    }


    public async Task<Result<UsuarioDto>> HandleAsync(CreateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var model = request.Usuario;
            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }


            // Verificar si el email ya existe
            var emailExists = await _context.Usuarios
                .AnyAsync(u => u.Email == model.Email, cancellationToken);

            if (emailExists)
            {
                return Result.Invalid(new ValidationError("El email ya está registrado"));
            }

            // Verificar si el nombre de usuario ya existe
            var usernameExists = await _context.Usuarios
                .AnyAsync(u => u.UserName == model.UsuarioNombre, cancellationToken);

            if (usernameExists)
            {
                return Result.Invalid(new ValidationError("El nombre de usuario ya está registrado"));
            }

            // Verificar que el rol existe
            var rolExists = await _context.Roles
                .AnyAsync(r => r.IdRol == model.RolId, cancellationToken);

            if (!rolExists)
            {
                return Result.Invalid(new ValidationError("El rol especificado no existe"));
            }


            var usuario = _mapper.Map<Usuario>(request.Usuario);


            usuario.IdGenero = 1;
            usuario.Activo = true;
            usuario.FechaRegistracion = DateTime.Now;
            usuario.PerteneceAlComite = false;
            usuario.EsCoordinador = false;
            usuario.PermiteSeleccionarLineaCredito = false;
            usuario.PermiteEdicionAltaContrato = false;
            usuario.PermiteSupervisionAltaContrato = false;
            usuario.PermiteAltaContrato = false;

            // Hash de la contraseña (aquí deberías usar BCrypt o similar)
            usuario.UserPass = BCrypt.Net.BCrypt.HashPassword(request.Usuario.Contrasena);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Result.Success(usuarioDto, "Usuario creado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al crear el usuario: {ex.InnerException}");
        }
    }
}
