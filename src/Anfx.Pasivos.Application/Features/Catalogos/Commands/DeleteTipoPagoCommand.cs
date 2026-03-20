namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class DeleteTipoPagoCommand : ICommand<Result>
{
    public int IdTipoPago { get; set; }
}

internal class DeleteTipoPagoCommandHandler : ICommandHandler<DeleteTipoPagoCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteTipoPagoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> HandleAsync(DeleteTipoPagoCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var tipoPago = await _context.PSV_TipoPago
                .Include(t => t.PSV_Pago) 
                .SingleOrDefaultAsync(r => r.IdTipoPago == message.IdTipoPago, cancellationToken);

            if (tipoPago == null)
                return Result.NotFound("El tipo de pago no fue encontrado");


            // Validar si tiene pagos asociados (si aplica)
            if (tipoPago.PSV_Pago != null && tipoPago.PSV_Pago.Any())
            {
                var pagos = string.Join(", ", tipoPago.PSV_Pago.Take(3).Select(p => p.IdPago));
                var totalPagos = tipoPago.PSV_Pago.Count;
                var mensaje = totalPagos > 3
                    ? $"No se puede eliminar el tipo de pago porque tiene {totalPagos} pagos asociados (ej: {pagos}, ...)"
                    : $"No se puede eliminar el tipo de pago porque tiene pagos asociados: {pagos}";

                return Result.Invalid(new ValidationError(mensaje));
            }

            _context.PSV_TipoPago.Remove(tipoPago);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("El tipo de pago fue eliminado correctamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al eliminar el tipo de pago: {ex.Message}");
        }
    }
}