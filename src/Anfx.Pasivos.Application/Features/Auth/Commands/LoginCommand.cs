using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public record LoginCommand : ICommand<Result<UsuarioLoginDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Contrasenia { get; set; } = string.Empty;
}
