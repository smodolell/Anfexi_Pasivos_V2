using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(UsuarioLoginDto user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        UsuarioLoginDto? GetUserFromToken(string token);
    }
}
