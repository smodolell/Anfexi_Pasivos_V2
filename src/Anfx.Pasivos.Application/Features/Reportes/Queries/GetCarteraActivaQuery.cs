using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetCarteraActivaQuery : IQuery<Result<CarteraDto>>
{
    public int? IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int? IdContratoActivo { get; set; }
    public int? Saldos { get; set; }
}
internal class GetCarteraActivaQueryHandler : IQueryHandler<GetCarteraActivaQuery, Result<CarteraDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCarteraActivaQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<CarteraDto>> HandleAsync(GetCarteraActivaQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Procedures.usp_CarteraActiva_CIAsync(
            request.IdFondeador,
            request.IdContratoPasivo,
            request.IdContratoActivo,
            request.Saldos
        );
        var result = data.Select(x => new CarteraDto
        {
            Capital = x.Capital,
            Interes = x.Interes
        }).FirstOrDefault();

        if (result == null)
        {
            return Result.NotFound("No hay resultados");
        }

        return Result.Success(result);
    }
}

