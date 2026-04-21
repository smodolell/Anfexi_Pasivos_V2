namespace Anfx.Pasivos.Application.Features.Reportes.Commands;

public class DeleteReporteCommand : ICommand<Result>
{
    public int ReporteId { get; set; }
}

public class DeleteReporteCommandHandler(
    IApplicationDbContext context
) : ICommandHandler<DeleteReporteCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteReporteCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var oReporte = await _context.RSP_Reporte
                .Include(i => i.RSP_Parametro)
                .SingleOrDefaultAsync(r => r.Id == message.ReporteId, cancellationToken);

            if (oReporte == null)
                return Result.NotFound($"No se encontró el reporte con Id {message.ReporteId}.");

            _context.RSP_Parametro.RemoveRange(oReporte.RSP_Parametro);
            _context.RSP_Reporte.Remove(oReporte);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
