namespace Anfx.Pasivos.Application.Features.Reportes.Commands;

public class DeleteArchivoCommand : ICommand<Result>
{
    public Guid ArchivoId { get; set; }
}

internal class DeleteArchivoCommandHandler(
    IApplicationDbContext context
) : ICommandHandler<DeleteArchivoCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteArchivoCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var archivo = await _context.RSP_Archivo
                .SingleOrDefaultAsync(a => a.Id == message.ArchivoId, cancellationToken);

            if (archivo == null)
                return Result.NotFound("El archivo no existe.");

            if (File.Exists(archivo.MapPath))
                File.Delete(archivo.MapPath);

            _context.RSP_Archivo.Remove(archivo);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
