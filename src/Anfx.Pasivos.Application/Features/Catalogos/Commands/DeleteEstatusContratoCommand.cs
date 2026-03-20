public class DeleteEstatusContratoCommand : ICommand<Result>
{
    public int IdEstatusContrato { get; set; }
}

internal class DeleteEstatusContratoCommandHandler : ICommandHandler<DeleteEstatusContratoCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private static readonly HashSet<int> EstatusProtegidos = new() { 1, 2, 3, 4 };

    public DeleteEstatusContratoCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }

    public async Task<Result> HandleAsync(DeleteEstatusContratoCommand message, CancellationToken cancellationToken = default)
    {
        try
        {
            if (EstatusProtegidos.Contains(message.IdEstatusContrato))
            {
                var nombreEstatus = await _context.PSV_EstatusContrato
                    .Where(e => e.IdEstatusContrato == message.IdEstatusContrato)
                    .Select(e => e.EstatusContrato)
                    .FirstOrDefaultAsync(cancellationToken);

                return Result.Invalid(new ValidationError(
                    $"No se puede eliminar el estatus '{nombreEstatus ?? message.IdEstatusContrato.ToString()}' porque es un estatus crítico del sistema."));
            }

            var estatus = await _context.PSV_EstatusContrato
                .SingleOrDefaultAsync(r => r.IdEstatusContrato == message.IdEstatusContrato, cancellationToken);

            if (estatus == null)
                return Result.NotFound("El estatus de contrato no fue encontrado");



            _context.PSV_EstatusContrato.Remove(estatus);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("El estatus de contrato fue desactivado correctamente");
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al desactivar el estatus de contrato: {ex.Message}");
        }
    }
}