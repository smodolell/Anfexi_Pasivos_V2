namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class DeleteTasaVariableCommand : ICommand<Result>
{
    public int IdTasa { get; set; }
}

internal class DeleteTasaVariableCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTasaVariableCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteTasaVariableCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.Tasa
                .Include(t => t.TasaValors)
                .Include(t => t.PSV_Contrato)
                .Include(t => t.PSV_Contrato1)
                .Include(t => t.PSV_LineaCredito)
                .SingleOrDefaultAsync(t => t.IdTasa == request.IdTasa && t.EsVariable == true, cancellationToken);

            if (entity == null)
            {
                return Result.NotFound("Tasa variable no encontrada");
            }

            if (entity.PSV_Contrato.Any() || entity.PSV_Contrato1.Any() || entity.PSV_LineaCredito.Any())
            {
                return Result.Invalid(new ValidationError("No se puede eliminar la tasa porque tiene contratos o líneas de crédito asociados"));
            }

            if (entity.TasaValors.Any())
            {
                _context.TasaValor.RemoveRange(entity.TasaValors);
            }

            _context.Tasa.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("La tasa variable fue eliminada");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
