using Anfx.Pasivos.Application.Features.Auth.DTOs;

namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public record UpdateRolCommand(int Id, RolUpdateDto Model) : ICommand<Result<RolDto>>;
