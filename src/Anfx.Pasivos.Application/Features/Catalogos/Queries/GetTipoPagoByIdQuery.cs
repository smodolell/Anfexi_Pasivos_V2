using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetTipoPagoByIdQuery : IQuery<Result<TipoPagoDto>>
{
    public int Id { get; set; }
}

internal class GetTipoPagoByIdQueryHandler : IQueryHandler<GetTipoPagoByIdQuery, Result<TipoPagoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTipoPagoByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<TipoPagoDto>> HandleAsync(GetTipoPagoByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PSV_TipoPago.SingleOrDefaultAsync(r => r.IdTipoPago == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tipo de Pago no existe");
            }
            var dto = _mapper.Map<TipoPagoDto>(entity);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
