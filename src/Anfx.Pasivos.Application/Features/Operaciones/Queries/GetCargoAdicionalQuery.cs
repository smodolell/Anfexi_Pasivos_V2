using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetCargoAdicionalQuery : IQuery<Result<CargoAdicionalViewDto>>
{
    public string? ContratoPasivo { get; set; }
}

internal class GetCargoAdicionalQueryHandler(IApplicationDbContext context, IDatabaseService databaseService, IMapper mapper) : IQueryHandler<GetCargoAdicionalQuery, Result<CargoAdicionalViewDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDatabaseService _databaseService = databaseService;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<CargoAdicionalViewDto>> HandleAsync(GetCargoAdicionalQuery message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.ContratoPasivo))
        {
            return Result.NotFound("No se estableció la clave de Contrato.");
        }

        var contratoPasivo = message.ContratoPasivo.Split(new[] { " - " }, StringSplitOptions.None)[0];

        var itemDb = await _context.PSV_Contrato
            .Include(r => r.PSV_Movimiento)
            .Include(r => r.PSV_EstatusContrato)
            .Include(r => r.PSV_TipoCredito)
            .Include(r => r.SB_Periodicidad)
            .Include(r => r.SB_TipoMoneda)
            .FirstOrDefaultAsync(f => f.Contrato == contratoPasivo);

        if (itemDb == null)
        {
            return Result.NotFound("El contrato al que se hace referencia no fue encontrado.");
        }

        var result = _mapper.Map<CargoAdicionalViewDto>(itemDb);


        result.Movimientos = await _databaseService.GetDetalleCargosAsync(itemDb.IdContrato);

        var movs = await _databaseService.GetDetalleMovimientosAsync(itemDb.IdContrato);
        var hoy = DateTime.Now.Date;

        result.SaldoVencido = itemDb.PSV_Movimiento
            .Where(w => w.FecMovimiento <= hoy && w.SaldoTotal > 0)
            .Sum(s => (decimal?)s.SaldoTotal) ?? 0;

        result.SaldoInsoluto += movs.Where(w => w.EsRenta).Sum(s => (decimal?)s.SaldoCapital) ?? 0;


        return Result.Success(result);
    }
}



public class GetCargoAdicionalByIdQuery : IQuery<Result<CargoAdicionalDto>>
{
    public int IdMovimiento { get; set; }
}


public class GetCargoAdicionalByIdQueryHandler(IApplicationDbContext context, IMapper mapper) : IQueryHandler<GetCargoAdicionalByIdQuery, Result<CargoAdicionalDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<CargoAdicionalDto>> HandleAsync(GetCargoAdicionalByIdQuery message, CancellationToken cancellationToken = default)
    {
        var movimiento = await _context.PSV_Movimiento.FirstOrDefaultAsync(f => f.IdMovimiento == message.IdMovimiento, cancellationToken);
        if (movimiento == null) return Result.NotFound("Movimiento no encontrado");

        var result = _mapper.Map<CargoAdicionalDto>(movimiento);

        result.PorcIVA = 0.16m;

        return Result.Success(result);
    }
}
