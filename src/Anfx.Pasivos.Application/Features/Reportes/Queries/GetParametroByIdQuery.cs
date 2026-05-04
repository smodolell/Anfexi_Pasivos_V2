using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetParametroByIdQuery : IQuery<Result<ParametroEditDto>>
{
    public Guid ParametroId { get; set; }
}

internal class GetParametroByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IQueryHandler<GetParametroByIdQuery, Result<ParametroEditDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<ParametroEditDto>> HandleAsync(GetParametroByIdQuery message, CancellationToken cancellationToken = default)
    {
        var oParametro = await _context.RSP_Parametro
            .SingleOrDefaultAsync(r => r.Id == message.ParametroId, cancellationToken);

        if (oParametro == null)
            return Result.NotFound("El parámetro no existe.");

        var result = _mapper.Map<ParametroEditDto>(oParametro);
        result.TablaRef ??= "";
        return Result.Success(result);
    }
}
