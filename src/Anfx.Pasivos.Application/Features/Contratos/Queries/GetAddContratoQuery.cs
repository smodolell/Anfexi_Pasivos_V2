using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetAddContratoQuery : IQuery<Result<ContratoPasivoEditDto>>
{
    public int IdLineaCredito { get; set; }
}



internal class GetAddContratoQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAddContratoQuery, Result<ContratoPasivoEditDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ContratoPasivoEditDto>> HandleAsync(GetAddContratoQuery message, CancellationToken cancellationToken = default)
    {
        var lineaCredito = await _context.PSV_LineaCredito
            .Include(i => i.PSV_Fondeador)
            .Include(i => i.Tasa1)
            .SingleOrDefaultAsync(r => r.IdLineaCredito == message.IdLineaCredito);

        if (lineaCredito == null)
            return Result.Invalid(new ValidationError("No existe la linea de Credito"));

        var result = new ContratoPasivoEditDto
        {
            IdFondeador = lineaCredito.IdFondeador,
            Fondeador = lineaCredito.PSV_Fondeador.Fondeador,
            MaxCapitalDisponible = lineaCredito.MontoDisponible,
            IdMoneda = lineaCredito.IdMoneda,
            TipoTasa = lineaCredito.Tasa1.EsVariable,
            IdTasa = lineaCredito.IdTasa!.Value,
            Tasa = lineaCredito.Tasa ?? 0,
            EstatusContrato = "CAPTURADO",
            LineaCredito = string.Format("ID [{0}] -> $ {1:N2}, Disponible: $ {2:N2}", lineaCredito.IdLineaCredito, lineaCredito.MontoAprobado, lineaCredito.MontoDisponible)
        };


        return Result.Success(result);
    }
}
