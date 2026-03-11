namespace Anfx.Pasivos.Application.Features.Configuracion.Commands;

public class DeleteLineaCreditoCommand : ICommand<Result>
{
    public int Id { get; set; }
}

internal class DeleteLineaCreditoCommandHandler(
    IApplicationDbContext context
) : ICommandHandler<DeleteLineaCreditoCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteLineaCreditoCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var lineaCredito = await _context.PSV_LineaCredito
                .SingleOrDefaultAsync(x => x.IdLineaCredito == request.Id, cancellationToken);

            if (lineaCredito == null)
            {
                return Result.NotFound("Línea de crédito no encontrada");
            }

            _context.PSV_LineaCredito.Remove(lineaCredito);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}