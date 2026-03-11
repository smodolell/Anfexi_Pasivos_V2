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
    public async Task<Result<List<RelLineaCreditoTipoCreditoDto>>> HandleAsync(GetTiposCreditoByIdLineaCreditoQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await _context.PSV_TipoCredito
                .Where(x => x.Activo)
                .ToListAsync(cancellationToken);
            var result = _mapper.Map<List<RelLineaCreditoTipoCreditoDto>>(entities);




            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}


