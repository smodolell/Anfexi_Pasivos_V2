using Anfx.Pasivos.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Anfx.Pasivos.Infrastructure.Services;


public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var claimValue = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimValue, out var id) ? id : null;
        }
    }
    public string? UserName => User?.FindFirst(ClaimTypes.Name)?.Value ?? "Invitado";

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value??"soporte@anfexi.com";

    public string? Role => User?.FindAll(ClaimTypes.Role).LastOrDefault()?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}