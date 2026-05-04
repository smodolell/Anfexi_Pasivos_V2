namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class DeleteTasaFijaCommand : ICommand<Result>
{
    public int IdTasa { get; set; }
}

internal class DeleteTasaFijaCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTasaFijaCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteTasaFijaCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.Tasa
                .Include(t => t.PSV_Contrato)
                .Include(t => t.PSV_Contrato1)
                .Include(t => t.PSV_LineaCredito)
                .SingleOrDefaultAsync(t => t.IdTasa == request.IdTasa && t.EsVariable == false, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Tasa fija no encontrada");
            }

            if (entity.PSV_Contrato.Any() || entity.PSV_Contrato1.Any() || entity.PSV_LineaCredito.Any())
            {
                return Result.Invalid(new ValidationError("No se puede eliminar la tasa porque tiene contratos o líneas de crédito asociados"));
            }

            _context.Tasa.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("La tasa fija fue eliminada");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
