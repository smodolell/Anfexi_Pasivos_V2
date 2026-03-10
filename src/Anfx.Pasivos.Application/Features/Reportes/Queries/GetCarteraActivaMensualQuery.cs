using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetCarteraActivaMensualQuery : IQuery<Result<List<CarteraMensualDto>>>
{
    public int? IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int? IdContratoActivo { get; set; }
    public int? Saldos { get; set; }
}


internal class GetCarteraActivaMensualQueryHandler : IQueryHandler<GetCarteraActivaMensualQuery, Result<List<CarteraMensualDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCarteraActivaMensualQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<CarteraMensualDto>>> HandleAsync(GetCarteraActivaMensualQuery message, CancellationToken cancellationToken = default)
    {
        var data = await _context.Procedures.usp_CarteraActivaMensual_CIAsync(
            message.IdFondeador,
            message.IdContratoPasivo,
            message.IdContratoActivo,
            message.Saldos);


        var result = data.Select(x => new CarteraMensualDto
        {
            Id = x.Id,
            FecIni = x.FecIni,
            FecFin = x.FecFin,
            Capital = x.Capital,
            Interes = x.Interes
        }).ToList();


        return Result.Success(result);

    }
}