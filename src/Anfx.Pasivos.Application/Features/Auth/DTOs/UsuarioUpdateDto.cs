namespace Anfx.Pasivos.Application.Features.Auth.DTOs;

public class UsuarioUpdateDto
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UsuarioNombre { get; set; } = string.Empty;

    public string? Contrasena { get; set; }

    public int RolId { get; set; }
}
