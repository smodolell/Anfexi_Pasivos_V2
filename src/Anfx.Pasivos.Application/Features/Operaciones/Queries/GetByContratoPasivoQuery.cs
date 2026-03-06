using Anfx.Pasivos.Application.Features.Operaciones.DTOs;
using Microsoft.VisualBasic;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetByContratoPasivoQuery :IQuery<Result<InfoGeneralContratoPasivoDto>>
{
    public string ContratoPasivo { get; set; }
}


internal class GetByContratoPasivoQueryHandler : IQueryHandler<GetByContratoPasivoQuery, Result<InfoGeneralContratoPasivoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetByContratoPasivoQueryHandler(IApplicationDbContext context,IMapper mapper)
    {
        this._context = context;
        this._mapper = mapper;
    }
    public async Task<Result<InfoGeneralContratoPasivoDto>> HandleAsync(GetByContratoPasivoQuery message, CancellationToken cancellationToken = default)
    {

        message.ContratoPasivo = message.ContratoPasivo.Split(new[] { " - " }, StringSplitOptions.None)[0];
        var itemDb = await _context.PSV_Contrato
            .FirstOrDefaultAsync(f => f.Contrato == message.ContratoPasivo && f.IdEstatusContrato == 2);

        if (itemDb == null)
        {
            return Result.NotFound("El contrato al que se hace referencia no fue encontrado.");
        }

        var SaldoInsoluto = itemDb.PSV_TablaAmortiza.Where(w => !w.Procesado && w.VersionTabla == w.PSV_Contrato.VersionTabla && w.IdTipoTabla == 1)
               .Sum(s => (decimal?)s.Capital) ?? 0;

        if (SaldoInsoluto == 0)
        {
            return Result.Invalid(new ValidationError("El contrato Pasivo ya no tiene Vencimientos por procesar, para aplicar el anticipo, no se puede continuar."));
        }

        var model = _mapper.Map(itemDb, new InfoGeneralContratoPasivoDto());
        model.IdTipoReduccion = 1;
        var hoy = DateTime.Now.Date;

        //model.TablaAmortiza = db.Database.SqlQuery<TablaAmortizaItemDto>(TempQuery.DetalleTablaAmortiza, model.IdContratoPasivo, itemDb.VersionTabla, 1).ToList();

        //model.SaldoVencido = db.PSV_Movimiento.Where(w => w.IdContrato == model.IdContratoPasivo && w.FecMovimiento <= hoy && w.SaldoTotal > 0)
        //    .Sum(s => (decimal?)s.SaldoTotal) ?? 0;

        //model.Movimientos = db.Database.SqlQuery<MovimientoItemDto>(TempQuery.DetalleMovimientos, model.IdContratoPasivo).ToList();

        //model.SaldoInsoluto = SaldoInsoluto + model.Movimientos.Where(w => w.EsRenta).Sum(s => (decimal?)s.SaldoCapital) ?? 0;

        //model.Pagos = db.Database.SqlQuery<PagoItemDto>(TempQuery.DetallePagos, model.IdContratoPasivo).ToList();

        //model.FechaAnticipo = DateTime.Now;

        //var ta = model.TablaAmortiza.Where(w => !w.Procesado && w.IdTablaAmortiza != -1).OrderBy(o => o.NoPago).FirstOrDefault();

        //if (ta != null)
        //{
        //    model.FechaAnticipo = ta.FecInicial.Value;
        //}
        return Result.Success(model);
    }
}