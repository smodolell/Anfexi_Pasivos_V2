using Anfx.Pasivos.Application.Features.Catalogos.DTOs;

namespace Anfx.Pasivos.Application.Features.Catalogos.Queries;

public class GetCuentaBancariaByIdQuery : IQuery<Result<CuentaBancariaDto>>
{
    public int Id { get; set; }
}

internal class GetCuentaBancariaByIdQueryHandler : IQueryHandler<GetCuentaBancariaByIdQuery, Result<CuentaBancariaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCuentaBancariaByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CuentaBancariaDto>> HandleAsync(GetCuentaBancariaByIdQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _context.PSV_CuentaBancaria.SingleOrDefaultAsync(r => r.IdCuentaBancaria == request.Id, cancellationToken);
            if (entity == null)
            {
                return Result.NotFound("Cuenta Bancaria no existe");
            }
            var dto = _mapper.Map<CuentaBancariaDto>(entity);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
