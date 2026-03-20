namespace Anfx.Pasivos.Application.Common.Interfaces;

public interface IUserContext
{
    int? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
