namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class DeleteCuentaBancariaCommand : ICommand<Result>
{
    public int IdCuentaBancaria { get; set; }
}

internal class DeleteCuentaBancariaCommandHandler : ICommandHandler<DeleteCuentaBancariaCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteCuentaBancariaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> HandleAsync(DeleteCuentaBancariaCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            var cuentaBancaria = await _context.PSV_CuentaBancaria
                .Include(c => c.PSV_Banco) // Incluir banco relacionado
                .Include(c => c.PSV_Pago) // Incluir pagos relacionados
                .SingleOrDefaultAsync(r => r.IdCuentaBancaria == message.IdCuentaBancaria, cancellationToken);

            if (cuentaBancaria == null)
                return Result.NotFound("La cuenta bancaria no fue encontrada");

            // Validar si tiene pagos asociados
            if (cuentaBancaria.PSV_Pago != null && cuentaBancaria.PSV_Pago.Any())
            {
                var pagos = string.Join(", ", cuentaBancaria.PSV_Pago.Take(3).Select(p => p.IdPago));
                var totalPagos = cuentaBancaria.PSV_Pago.Count;
                var mensaje = totalPagos > 3
                    ? $"No se puede eliminar la cuenta bancaria porque tiene {totalPagos} pagos asociados (ej: {pagos}, ...)"
                    : $"No se puede eliminar la cuenta bancaria porque tiene pagos asociados: {pagos}";

                return Result.Invalid(new ValidationError(mensaje));
            }

            

            // Registrar información de auditoría antes de eliminar
            var nombreBanco = cuentaBancaria.PSV_Banco?.Banco ?? "Desconocido";
            var numeroCuenta = cuentaBancaria.CuentaBancaria;

            _context.PSV_CuentaBancaria.Remove(cuentaBancaria);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage($"La cuenta bancaria {numeroCuenta} del banco {nombreBanco} fue eliminada correctamente");
        }
        catch (DbUpdateException ex)
        {
            // Capturar errores de integridad referencial
            return Result.Error("No se puede eliminar la cuenta bancaria porque tiene registros relacionados en otras tablas");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al eliminar la cuenta bancaria: {ex.Message}");
        }
    }
}