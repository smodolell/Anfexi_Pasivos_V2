using System.IO;
using System.Security.Cryptography;
using System.Text;
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
            (!string.IsNullOrEmpty(request.Email) && u.Email == request.Email) || (!string.IsNullOrEmpty(request.Usuario) && u.UserName == request.Usuario),

        cancellationToken);

        if (usuario == null || (!usuario.Activo ?? false))
        {
            return Result.Unauthorized("Credenciales inválidas");
        }

        bool isPasswordValid = VerifyPassword(request.Contrasenia, usuario.UserPass);


        if (!isPasswordValid)
        {
            return Result.Unauthorized("Credenciales inválidas");
        }


        var loginResponse = new UsuarioLoginDto
        {
            Id = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto ?? "",
            Email = usuario.Email ?? "",
            UsuarioNombre = usuario.UserName,
            Role = usuario.Rol.Titulo ?? "",
            RefreshToken = _jwtService.GenerateRefreshToken(),
            TokenExpiration = DateTime.UtcNow.AddMinutes(60)
        };

        // Generar JWT token usando el servicio real
        loginResponse.Token = _jwtService.GenerateToken(loginResponse);

        return Result.Success(loginResponse, "Login exitoso");
    }

    /// <summary>
    /// Soporta contraseñas BCrypt (usuarios nuevos) y DES legacy (usuarios migrados).
    /// </summary>
    private static bool VerifyPassword(string plain, string? stored)
    {
        if (string.IsNullOrEmpty(stored)) return false;

        // BCrypt: empieza con $2a$, $2b$ o $2y$
        if (stored.StartsWith("$2"))
            return BCrypt.Net.BCrypt.Verify(plain, stored);

        // Legacy: DES con clave "Anfexi12"
        try
        {
            var key = Encoding.ASCII.GetBytes("Anfexi12");
            using var provider  = new DESCryptoServiceProvider();
            using var ms        = new MemoryStream(Convert.FromBase64String(stored));
            using var cs        = new CryptoStream(ms, provider.CreateDecryptor(key, key), CryptoStreamMode.Read);
            using var reader    = new StreamReader(cs);
            return reader.ReadToEnd() == plain;
        }
        catch
        {
            return false;
        }
    }
}

//public class LoginCommandDummyHandler : ICommandHandler<LoginCommand, Result<UsuarioLoginDto>>
//{
//    private readonly IJwtService _jwtService;

//    public LoginCommandDummyHandler(IJwtService jwtService)
//    {
//        _jwtService = jwtService;
//    }

//    public async Task<Result<UsuarioLoginDto>> HandleAsync(LoginCommand request, CancellationToken cancellationToken = default)
//    {
//        // Credenciales hardcodeadas
//        const string emailValido = "lalo.ariasr@gmail.com";
//        const string usuarioValido = "lalo.ariasr";
//        const string passwordValida = "default";

//        // Validar credenciales
//        bool credencialesValidas =
//            (request.Email == emailValido || request.Usuario == usuarioValido) &&
//            request.Contrasenia == passwordValida;

//        if (!credencialesValidas)
//        {
//            return Result.Unauthorized("Credenciales inválidas");
//        }

//        // Datos fijos del usuario dummy
//        var loginResponse = new UsuarioLoginDto
//        {
//            Id = 1,
//            NombreCompleto = "Eduardo Arias",
//            Email = emailValido,
//            UsuarioNombre = usuarioValido,
//            Role = "Admin",
//            RefreshToken = _jwtService.GenerateRefreshToken(),
//            TokenExpiration = DateTime.UtcNow.AddMinutes(60)
//        };

//        // Generar JWT token
//        loginResponse.Token = _jwtService.GenerateToken(loginResponse);

//        return Result.Success(loginResponse, "Login exitoso");
//    }
//}