namespace Anfx.Pasivos.Application.Features.Catalogos.Commands;

public class ToggleTasaStatusCommand : ICommand<Result>
{
    public int IdTasa { get; set; }
    public bool Activar { get; set; }
}

internal class ToggleTasaStatusCommandHandler : ICommandHandler<ToggleTasaStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public ToggleTasaStatusCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }

    public async Task<Result> HandleAsync(ToggleTasaStatusCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
           

            // Buscar la tasa
            var tasa = await _context.Tasa
                .Include(t => t.TasaValors)
                .SingleOrDefaultAsync(t => t.IdTasa == request.IdTasa, cancellationToken);

            if (tasa == null)
            {
                return Result.NotFound($"No se encontró la tasa con ID {request.IdTasa}");
            }

            // Validar si se puede desactivar (opcional)
            if (!request.Activar && !SePuedeDesactivar(tasa))
            {
                return Result.Invalid(new ValidationError("No se puede desactivar la tasa porque tiene registros asociados activos"));
            }

            // Cambiar el estado
            tasa.Activo = request.Activar;

            _context.Tasa.Update(tasa);
            await _context.SaveChangesAsync(cancellationToken);

            var mensaje = request.Activar ? "activada" : "desactivada";
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al cambiar el estado de la tasa: {ex.Message}");
        }
    }

    private bool SePuedeDesactivar(Tasa tasa)
    {
        // Validar si la tasa tiene contratos u otras relaciones activas
        if (tasa.PSV_Contrato?.Any() == true ||
            tasa.PSV_LineaCredito?.Any() == true)
        {
            return false;
        }
        return true;
    }
}