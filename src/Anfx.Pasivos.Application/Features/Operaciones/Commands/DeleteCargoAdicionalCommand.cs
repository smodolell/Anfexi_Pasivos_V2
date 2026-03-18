namespace Anfx.Pasivos.Application.Features.Operaciones.Commands;

public class DeleteCargoAdicionalCommand : ICommand<Result>
{
    public int IdMovimiento { get; set; }
}


internal class DeleteCargoAdicionalCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteCargoAdicionalCommand, Result>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> HandleAsync(DeleteCargoAdicionalCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var movimiento = await _context.PSV_Movimiento
                .Include(i => i.PSV_RelPagoMovimiento)
                .SingleOrDefaultAsync(r => r.IdMovimiento == message.IdMovimiento, cancellationToken);
            if (movimiento == null) return Result.NotFound("Movimiento no encontrado");

            if (movimiento.PSV_RelPagoMovimiento != null && movimiento.PSV_RelPagoMovimiento.Any())
            {
                return Result.Invalid(new ValidationError("No se puede eliminar un movimiento que tiene pagos aplicados"));
            }

            _context.PSV_Movimiento.Remove(movimiento);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("Eliminación realizada Correctamente");
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}

