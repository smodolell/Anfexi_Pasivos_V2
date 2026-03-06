using Anfx.Pasivos.Application.Features.Configuracion.Dtos;

namespace Anfx.Pasivos.Application.Features.Configuracion.Queries;

public class GetTipoTablaAmortizaByIdQuery : IQuery<Result<TipoTablaAmortizaDto>>
{
    public int Id { get; set; }
}

internal class GetTipoTablaAmortizaByIdQueryHandler : IQueryHandler<GetTipoTablaAmortizaByIdQuery, Result<TipoTablaAmortizaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTipoTablaAmortizaByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<TipoTablaAmortizaDto>> HandleAsync(GetTipoTablaAmortizaByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PSV_TipoTablaAmortiza.SingleOrDefaultAsync(r => r.IdTipoTablaAmortiza == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Tipo de Tabla Amortiza no existe");
            }
            var dto = _mapper.Map<TipoTablaAmortizaDto>(entity);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
