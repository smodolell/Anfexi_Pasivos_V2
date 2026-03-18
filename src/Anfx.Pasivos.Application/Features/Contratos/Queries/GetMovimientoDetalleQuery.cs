namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetMovimientoDetalleQuery : IQuery<Result<MovimientoDetalleDto>>
{
    public int IdMovimiento { get; set; }
}

internal class GetMovimientoDetalleQueryHandler(IApplicationDbContext context, IDatabaseService databaseService) : IQueryHandler<GetMovimientoDetalleQuery, Result<MovimientoDetalleDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDatabaseService _databaseService = databaseService;

    public async Task<Result<MovimientoDetalleDto>> HandleAsync(GetMovimientoDetalleQuery message, CancellationToken cancellationToken = default)
    {
        var movimiento  = await _context.PSV_Movimiento.SingleOrDefaultAsync(r => r.IdMovimiento == message.IdMovimiento);
        if (movimiento == null) return Result.NotFound("Movimiento no encontrado");
        
        var result = new MovimientoDetalleDto
        {
            IdMovimiento = movimiento.IdMovimiento,
            Detalle = await _databaseService.GetDetallePagosAplicadosAMovAsync(message.IdMovimiento)
        };

        return  Result.Success(result);
    }
}

