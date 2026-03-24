namespace Anfx.Pasivos.Application.Features.Auth.DTOs;

public class UsuarioItemDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string RolNombre { get; set; } = string.Empty;
}
