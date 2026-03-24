using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public record CreateRolCommand(RolCreateDto Model) : ICommand<Result<RolDto>>;
