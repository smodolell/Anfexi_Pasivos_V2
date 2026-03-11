using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetLineaCreditoByIdQuery : IQuery<Result<LineaCreditoDto>>
{
    public int Id { get; set; }
}

internal class GetLineaCreditoByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IQueryHandler<GetLineaCreditoByIdQuery, Result<LineaCreditoDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<LineaCreditoDto>> HandleAsync(GetLineaCreditoByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var lineaCredito = await _context.PSV_LineaCredito
                .Include(x => x.PSV_Fondeador)
                .Include(x => x.SB_TipoMoneda)
                .Include(x => x.Tasa)
                .SingleOrDefaultAsync(x => x.IdLineaCredito == request.Id, cancellationToken);

            if (lineaCredito == null)
            {
                return Result.NotFound("Línea de crédito no encontrada");
            }

            var dto = _mapper.Map<LineaCreditoDto>(lineaCredito);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}