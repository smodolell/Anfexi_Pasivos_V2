namespace Anfx.Pasivos.Application.Features.Auth.DTOs;

public class UserContextDto
{
    public int? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool IsAuthenticated { get; set; }

}
