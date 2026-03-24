namespace Anfx.Pasivos.Application.Features.Auth.DTOs;

public class UsuarioCreateDto
{
    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UsuarioNombre { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public int RolId { get; set; }
}
