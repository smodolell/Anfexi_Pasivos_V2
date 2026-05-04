using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetReporteByIdQuery : IQuery<Result<ReporteEditDto>>
{
    public int ReporteId { get; set; }
}

internal class GetReporteByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
) : IQueryHandler<GetReporteByIdQuery, Result<ReporteEditDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<ReporteEditDto>> HandleAsync(GetReporteByIdQuery message, CancellationToken cancellationToken = default)
    {
        var oReporte = await _context.RSP_Reporte
            .SingleOrDefaultAsync(r => r.Id == message.ReporteId, cancellationToken);

        if (oReporte == null)
            return Result.NotFound($"No se encontró el reporte con Id {message.ReporteId}.");

        var result = _mapper.Map<ReporteEditDto>(oReporte);
        return Result.Success(result);
    }
}
