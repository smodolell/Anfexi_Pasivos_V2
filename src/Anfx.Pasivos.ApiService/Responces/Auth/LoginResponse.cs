namespace Anfx.Pasivos.ApiService.Responces.Auth;

public record LoginResponse(bool Success, UserInfo User)
{
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
public record UserInfo(int Id)
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

