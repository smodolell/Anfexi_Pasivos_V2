using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Commands;

public record CreateRolCommand(RolCreateDto Model) : ICommand<Result<RolDto>>;
