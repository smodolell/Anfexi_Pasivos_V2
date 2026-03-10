using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetCarteraPasivaQuery : IQuery<Result<CarteraDto>>
{
    public int? IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int? IdContratoActivo { get; set; }
    public int? Saldos { get; set; }
}

internal class GetCarteraPasivaQueryHandler : IQueryHandler<GetCarteraPasivaQuery, Result<CarteraDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCarteraPasivaQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<CarteraDto>> HandleAsync(GetCarteraPasivaQuery request, CancellationToken cancellationToken)
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