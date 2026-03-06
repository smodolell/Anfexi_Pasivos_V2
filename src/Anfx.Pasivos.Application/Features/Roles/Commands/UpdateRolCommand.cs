using Anfx.Pasivos.Application.Features.Roles.DTOs;

namespace Anfx.Pasivos.Application.Features.Roles.Commands;

public record UpdateRolCommand(int Id, RolUpdateDto Model) : ICommand<Result<RolDto>>;
