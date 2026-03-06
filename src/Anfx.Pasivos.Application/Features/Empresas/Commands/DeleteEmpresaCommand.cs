namespace Anfx.Pasivos.Application.Features.Empresas.Commands;

public record DeleteEmpresaCommand(int Id) : ICommand<Result>;

public class DeleteEmpresaCommandHandler : ICommandHandler<DeleteEmpresaCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmpresaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> HandleAsync(DeleteEmpresaCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e => e.Id == message.Id, cancellationToken);

            if (empresa == null)
            {
                return Result.NotFound("Empresa no encontrada");
            }

            // Soft delete
            // Soft delete - marcar como eliminado (si tienes un campo Deleted o similar)
            // Por ahora, eliminamos físicamente
            _context.Empresas.Remove(empresa);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("Empresa eliminada exitosamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al eliminar la empresa: {ex.Message}");
        }
    }
}
