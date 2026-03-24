namespace Anfx.Pasivos.Application.Features.Auth.Commands;

public record DeleteRolCommand(int Id) : ICommand<Result<bool>>;

public class DeleteRolCommandHandler : ICommandHandler<DeleteRolCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteRolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(DeleteRolCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var rol = await _context.Roles
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.IdRol == request.Id, cancellationToken);

            if (rol == null)
            {
                return Result.NotFound("Rol no encontrado");
            }

            // Verificar si hay usuarios asignados al rol
            if (rol.Usuarios.Any())
            {
                return Result.Invalid(new ValidationError("No se puede eliminar el rol porque tiene usuarios asignados"));
            }

            // Soft delete
            //rol.Activo = false;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(true, "Rol eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al eliminar el rol: {ex.Message}");
        }
    }
}
