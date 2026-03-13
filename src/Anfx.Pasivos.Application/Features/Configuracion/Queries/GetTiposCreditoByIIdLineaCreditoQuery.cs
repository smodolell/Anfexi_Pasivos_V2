using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetTiposCreditoByIdLineaCreditoQuery : IQuery<Result<List<RelLineaCreditoTipoCreditoDto>>>
{
    public int IdLineaCredito { get; set; }
}


internal class GetTiposCreditoByIdLineaCreditoQueryHandler : IQueryHandler<GetTiposCreditoByIdLineaCreditoQuery, Result<List<RelLineaCreditoTipoCreditoDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetTiposCreditoByIdLineaCreditoQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Result<List<RelLineaCreditoTipoCreditoDto>>> HandleAsync(
       GetTiposCreditoByIdLineaCreditoQuery request,
       CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validar que existe la línea de crédito (opcional pero recomendado)
            var lineaCreditoExists = await _context.PSV_LineaCredito
                .AnyAsync(x => x.IdLineaCredito == request.IdLineaCredito, cancellationToken);

            if (!lineaCreditoExists)
            {
                return Result.NotFound($"Línea de crédito {request.IdLineaCredito} no encontrada");
            }

            // 2. Obtener todas las relaciones de una vez
            var relacionesDict = await _context.PSV_RelLineaCreditoTipoCredito
                .Where(x => x.IdLineaCredito == request.IdLineaCredito)
                .ToDictionaryAsync(x => x.IdTipoCredito, x => x.Seleccionado, cancellationToken);

            // 3. Obtener tipos de crédito activos
            var tiposCredito = await _context.PSV_TipoCredito
                .Where(x => x.Activo)
                .ToListAsync(cancellationToken);

            // 4. Mapear y asignar seleccionado
            var result = _mapper.Map<List<RelLineaCreditoTipoCreditoDto>>(tiposCredito);

            foreach (var item in result)
            {
                item.Seleccionado = relacionesDict.GetValueOrDefault(item.IdTipoCredito, false);
            }

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error al obtener tipos de crédito: {ex.Message}");
        }
    }
}


