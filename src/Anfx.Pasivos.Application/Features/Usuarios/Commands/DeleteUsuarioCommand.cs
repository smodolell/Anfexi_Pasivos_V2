using Azure.Core;

namespace Anfx.Pasivos.Application.Features.Usuarios.Commands;

public class DeleteUsuarioCommand : ICommand<Result<bool>>
{
    public int Id { get; set; }

    public DeleteUsuarioCommand(int id)
    {
        Id = id;
    }
}

public class DeleteUsuarioCommandHandler : ICommandHandler<DeleteUsuarioCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteUsuarioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Result<bool>> HandleAsync(DeleteUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (usuario == null)
            {
                return Result.NotFound("Usuario no encontrado");
            }

            // Soft delete - marcar como eliminado (si tienes un campo Deleted o similar)
            // Por ahora, eliminamos físicamente
            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(true, "Usuario eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error interno del servidor{ex.Message}");
        }
    }
}
