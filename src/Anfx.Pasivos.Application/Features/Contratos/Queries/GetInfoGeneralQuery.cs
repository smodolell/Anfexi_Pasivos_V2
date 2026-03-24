using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetInfoGeneralQuery : IQuery<Result<InfoGeneralContratoPasivoDto>>
{
    public string ContratoPasivo { get; set; } = "";
}





public class GetInfoGeneralQueryHandler : IQueryHandler<GetInfoGeneralQuery, Result<InfoGeneralContratoPasivoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDatabaseService _databaseService;

    public GetInfoGeneralQueryHandler(IApplicationDbContext context, IMapper mapper, IDatabaseService databaseService)
    {
        _context = context;
        _mapper = mapper;
        _databaseService = databaseService;
    }
    public async Task<Result<InfoGeneralContratoPasivoDto>> HandleAsync(GetInfoGeneralQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var contratoPasivo = request.ContratoPasivo.Split(new[] { " - " }, StringSplitOptions.None)[0];
            var contrato = await _context.PSV_Contrato
                .Include(i => i.SB_Periodicidad)
                .Include(i => i.SB_TipoMoneda)
                .Include(i => i.PSV_EstatusContrato)
                .Include(i => i.PSV_TipoCredito)
                .Include(i => i.PSV_TablaAmortiza)
                .Include(i => i.PSV_Fondeador)
                .FirstOrDefaultAsync(r => r.Contrato.Equals(contratoPasivo));
            if (contrato is null)
            {
                return Result.NotFound($"Contrato Clave[{contratoPasivo}] no encontrado");
            }

            var hoy = DateTime.Now.Date;

            var infoGeneral = _mapper.Map<InfoGeneralContratoPasivoDto>(contrato);

            infoGeneral.SaldoVencido = _context.PSV_Movimiento
                .Where(w => w.IdContrato == contrato.IdContrato && w.FecMovimiento <= hoy && w.SaldoTotal > 0)
                .Sum(s => (decimal?)s.SaldoTotal) ?? 0;

            infoGeneral.Movimientos = await _databaseService.GetDetalleMovimientosAsync(contrato.IdContrato);


            var SaldoInsoluto = contrato.PSV_TablaAmortiza
                .Where(w => !w.Procesado && w.VersionTabla == w.PSV_Contrato.VersionTabla && w.IdTipoTabla == 1)
                .Sum(s => (decimal?)s.Capital) ?? 0;

            infoGeneral.SaldoInsoluto = SaldoInsoluto + infoGeneral.Movimientos.Where(w => w.EsRenta).Sum(s => (decimal?)s.SaldoCapital) ?? 0;
            infoGeneral.Pagos = await _databaseService.GetDetallePagosAsync(contrato.IdContrato);

            return Result.Success(infoGeneral);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

    }

}



