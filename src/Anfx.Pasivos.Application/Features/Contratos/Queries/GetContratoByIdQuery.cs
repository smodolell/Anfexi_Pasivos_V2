using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetContratoByIdQuery : IQuery<Result<ContratoPasivoEditDto>>
{
    public int IdContrato { get; set; }
}

internal class GetContratoByIdQueryHandler(IApplicationDbContext context, IMapper mapper) : IQueryHandler<GetContratoByIdQuery, Result<ContratoPasivoEditDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<ContratoPasivoEditDto>> HandleAsync(GetContratoByIdQuery message, CancellationToken cancellationToken = default)
    {
     
        var contrato = await _context.PSV_Contrato
                .Include(i => i.SB_Periodicidad)
                .Include(i => i.SB_TipoMoneda)
                .Include(i => i.PSV_EstatusContrato)
                .Include(i => i.PSV_TipoCredito)
                .Include(i => i.PSV_TablaAmortiza)
                .Include(i => i.PSV_Fondeador)
                .Include(i => i.Tasa1)
                .Include(i => i.Tasa2)
                .SingleOrDefaultAsync(r => r.IdContrato == message.IdContrato, cancellationToken);

        if (contrato == null) return Result.NotFound("Contrato no encontrado");

        var result = _mapper.Map<ContratoPasivoEditDto>(contrato);
        var lineaCredito = await _context.PSV_RelLineaCreditoContrato
            .Include(i => i.PSV_LineaCredito)
            .Where(r => r.IdContrato == contrato.IdContrato)
            .Select(s => s.PSV_LineaCredito)
            .FirstOrDefaultAsync();

        if (lineaCredito != null)
        {
            result.IdLineaCredito = lineaCredito.IdLineaCredito;
            result.MaxCapitalDisponible = lineaCredito.MontoDisponible;
            result.LineaCredito = string.Format("ID [{0}] -> $ {1:N2}, Disponible: $ {2:N2}", lineaCredito.IdLineaCredito, lineaCredito.MontoAprobado, lineaCredito.MontoDisponible);
        }



        return Result.Success(result);
    }
}