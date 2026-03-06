using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetEstatusContratoByIdQuery : IQuery<Result<EstatusContratoDto>>
{
    public int Id { get; set; }
}

internal class GetEstatusContratoByIdQueryHandler : IQueryHandler<GetEstatusContratoByIdQuery, Result<EstatusContratoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEstatusContratoByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EstatusContratoDto>> HandleAsync(GetEstatusContratoByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PSV_EstatusContrato.SingleOrDefaultAsync(r => r.IdEstatusContrato == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Estatus de Contrato no existe");
            }
            var dto = _mapper.Map<EstatusContratoDto>(entity);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
