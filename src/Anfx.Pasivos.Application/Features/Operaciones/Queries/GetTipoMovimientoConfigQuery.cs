using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetTipoMovimientoConfigQuery : IQuery<Result<TipoMovimientoConfigDto>>
{
    public int IdTipoMovimiento { get; set; }
}


internal class GetTipoMovimientoConfigQueryHandler : IQueryHandler<GetTipoMovimientoConfigQuery, Result<TipoMovimientoConfigDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTipoMovimientoConfigQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TipoMovimientoConfigDto>> HandleAsync(GetTipoMovimientoConfigQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var itemDb = await _context.TipoMovimiento
                .FirstOrDefaultAsync(f => f.IdTipoMovimiento == message.IdTipoMovimiento, cancellationToken);

            if (itemDb == null)
            {
                return Result.NotFound("No se encontró el tipo de movimiento.");
            }

            var result = new TipoMovimientoConfigDto
            {
                GeneraIVA_Capital = itemDb.GeneraIVACapital == true ? 1 : 0,
                GeneraIVA_Interes = itemDb.GeneraIVAInteres == true ? 1 : 0
            };

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener configuración: {ex.Message}");
        }
    }
}