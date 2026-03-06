using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

internal class GetTipoCreditoByIdQueryHandler:IQueryHandler<GetTipoCreditoByIdQuery, Result<TipoCreditoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetTipoCreditoByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Result<TipoCreditoDto>> HandleAsync(GetTipoCreditoByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PSV_TipoCredito.SingleOrDefaultAsync(r => r.IdTipoCredito == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tipo de Credito no existe");
            }
            var dto = _mapper.Map<TipoCreditoDto>(entity);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
