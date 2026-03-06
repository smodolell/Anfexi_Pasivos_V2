using Anfx.Pasivos.Application.Features.Roles.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Anfx.Pasivos.Application.Features.Usuarios.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public int RolId { get; set; }
    public bool Activo { get; set; }
    public RolDto Rol { get; set; } = new RolDto();
}

public class UsuarioItemDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string RolNombre { get; set; } = string.Empty;
}

public class UsuarioCreateDto
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string UsuarioNombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    public int RolId { get; set; }
}

public class UsuarioUpdateDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string UsuarioNombre { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string? Contrasena { get; set; }

    [Required(ErrorMessage = "El rol es requerido")]
    public int RolId { get; set; }
}

public class UsuarioLoginResponseDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpiration { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class RefreshTokenDto
{
    [Required(ErrorMessage = "El refresh token es requerido")]
    public string RefreshToken { get; set; } = string.Empty;
}

public class TokenValidationDto
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public UsuarioLoginResponseDto? User { get; set; }
}

public class TokenValidationResultDto
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}

// Clases para AuthController
public class LoginRequestDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Contrasenia { get; set; } = string.Empty;
}

public class LoginByUsernameRequestDto
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string UsuarioNombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Contrasena { get; set; } = string.Empty;
}

// DTOs para UsuariosController
public class CreateUsuarioDto
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string UsuarioNombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    public int RolId { get; set; }
}

public class UpdateUsuarioDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(100, ErrorMessage = "El nombre completo no puede exceder 100 caracteres")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
    public string UsuarioNombre { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string? Contrasena { get; set; }

    [Required(ErrorMessage = "El rol es requerido")]
    public int RolId { get; set; }
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int Id { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string NuevaContrasena { get; set; } = string.Empty;
}
