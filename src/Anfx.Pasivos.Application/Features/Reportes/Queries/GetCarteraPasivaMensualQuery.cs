using Anfx.Pasivos.Application.Features.Reportes.DTOs;

namespace Anfx.Pasivos.Application.Features.Reportes.Queries;

public class GetCarteraPasivaMensualQuery : IQuery<Result<List<CarteraMensualDto>>>
{
    public int? IdFondeador { get; set; }
    public int? IdContratoPasivo { get; set; }
    public int? IdContratoActivo { get; set; }
    public int? Saldos { get; set; }
}


internal class GetCarteraPasivaMensualQueryHandler : IQueryHandler<GetCarteraPasivaMensualQuery, Result<List<CarteraMensualDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCarteraPasivaMensualQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<CarteraMensualDto>>> HandleAsync(GetCarteraPasivaMensualQuery message, CancellationToken cancellationToken = default)
    {
        var data = await _context.Procedures.usp_CarteraPasivaMensual_CIAsync(
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