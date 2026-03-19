using Anfx.Pasivos.Application.Features.Operaciones.DTOs;

namespace Anfx.Pasivos.Application.Features.Operaciones.Queries;

public class GetAnticipoConfigQuery : IQuery<Result<AnticipoConfigDto>>
{
    public int IdTipoTerminacion { get; set; }
    public int IdContrato { get; set; }
}
internal class GetAnticipoConfigQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAnticipoConfigQuery, Result<AnticipoConfigDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AnticipoConfigDto>> HandleAsync(GetAnticipoConfigQuery message, CancellationToken cancellationToken = default)
    {
        var itemDb = await _context.PSV_TipoTerminacion.FirstOrDefaultAsync(f => f.IdTipoTerminacion == message.IdTipoTerminacion);
        if (itemDb == null)
        {
            return Result.NotFound("No se ha encontrado el tipo de terminación referida.");
        }
        var pasivo = await _context.PSV_Contrato.SingleOrDefaultAsync(f => f.IdContrato == message.IdContrato);
        if (pasivo == null)
        {
            return Result.NotFound("Contrato no encontrado");
        }

        var tmInt = itemDb.TipoMovimiento1 ?? new TipoMovimiento { GeneraIVAInteres = false };
        var tmPena = itemDb.TipoMovimiento2 ?? new TipoMovimiento { GeneraIVACapital = false };
        var SaldoInsoluto = pasivo.PSV_TablaAmortiza
            .Where(w => !w.Procesado && w.VersionTabla == w.PSV_Contrato.VersionTabla && w.IdTipoTabla == 1)
            .Sum(s => (decimal?)s.Capital) ?? 0;

        var result = new AnticipoConfigDto
        {
            EsLiquidacion = itemDb.EsLiquidacionTotal,
            MontoAnticipo = itemDb.EsLiquidacionTotal ? SaldoInsoluto : 0,
            MontoTotal = itemDb.EsLiquidacionTotal ? (pasivo.SaldoInsoluto ?? 0) : 0,
            PorcIVA_Interes = (tmInt.GeneraIVAInteres != null && tmInt.GeneraIVAInteres.Value) ? 0.16m : 0,
            PorcIVA_Pena = (tmPena.GeneraIVACapital != null && tmPena.GeneraIVACapital.Value) ? 0.16m : 0,
            CalculaInteres = itemDb.PermiteCalculoInteres,
            PermitePena = itemDb.IdTipoMovimientoPena != null
        };

        return result;
    }
}
//     <div class="panel-body">
//                @using(Html.BeginForm("Save", "Anticipo", FormMethod.Post,
//                    new
//                    {
//Area = "Cobranza",
//                        id = "FormAnticipo",
//                        enctype = "multipart/form-data",
//                        ajaxForm = "true",
//                        ajaxForm_UploadElementId = "DivAjaxResultAnticipo",
//                        ajaxForm_HttpMethod = "POST",
//                    }))
//                {
//                    @Html.HiddenFor(m => m.IdContratoPasivo)
//                    @Html.HiddenFor(m => m.PorcIVA_Interes)
//                    @Html.HiddenFor(m => m.PorcIVA_Pena)
//                    @Html.HiddenFor(m => m.EsLiquidacion)
//                    @Html.HiddenFor(m => m.CalculaInteres)
//                    <div class="form-horizontal">
//                        <fieldset>
//                            <legend>Datos de Aplicación de Anticipo</legend>
//                            <div class="row">
//                                <div class="col-sm-5">
//                                    <div class="form-group">
//                                        <label class="col-sm-6 control-label">Fecha Anticipo: </label>
//                                        <div class="col-sm-6">
//                                            @Html.EditorFor(m => m.FechaAnticipo, "DateH", new { onchange = "LocalApp.CalculaInteres()" })
//                                        </div>
//                                    </div>
//                                </div>
//                                <div class="col-sm-7">
//                                    <div class="form-group">
//                                        <label class="col-sm-3 control-label">Anticipo: </label>
//                                        <div class="col-sm-9">
//                                            @Html.EditorFor(m => m.MontoAnticipo, "MoneyH", new { onchange = "LocalApp.CalculaInteres()", data_val_range = string.Format("[1 ~ {0:N2}]", Model.SaldoInsoluto), data_val_range_max = Model.SaldoInsoluto })
//                                        </div>
//                                    </div>
//                                </div>
//                            </div>
//                            <div class="row">
//                                <div class="col-sm-5">
//                                    <div class="form-group">
//                                        <label class="col-sm-6 control-label">Tipo: </label>
//                                        <div class="col-sm-6">
//                                            @Html.DropDownListFor(m => m.IdTipoTerminacion, Cat.Filtro(Model.IdTipoCredito).GetList("TipoTerminacionPorTC"), "[ SELECCIONE ]", new { onchange = "LocalApp.GetAnticipoConfig(this)", @class = "form-control" })
//                                        </div>
//                                    </div>
//                                </div>
//                                <div class="col-sm-7">
//                                    <div class="form-group">
//                                        <label class="col-sm-3 control-label">Interes: </label>
//                                        <div class="col-sm-9">
//                                            <div class="row">
//                                                <div class="col-sm-6">
//                                                    @Html.EditorFor(m => m.MontoInteres, "MoneyH", new { @readonly = "readonly" })
//                                                </div>
//                                                <div class="col-sm-6">
//                                                    @Html.EditorFor(m => m.MontoIVA_Interes, "MoneyH", new { @readonly = "readonly" })
//                                                </div>
//                                            </div>
//                                        </div>
//                                    </div>
//                                </div>
//                            </div>
//                            <div class="row">
//                                <div class="col-sm-5">

//                                </div>
//                                <div class="col-sm-7">
//                                    <div class="form-group" id="DivPena" style="display: none">
//                                        <label class="col-sm-3 control-label">Pena: </label>
//                                        <div class="col-sm-9">
//                                            <div class="row">
//                                                <div class="col-sm-6">
//                                                    @Html.EditorFor(m => m.MontoPena, "MoneyH", new { onchange = "LocalApp.CalculaPena()" })
//                                                </div>
//                                                <div class="col-sm-6">
//                                                    @Html.EditorFor(m => m.MontoIVA_Pena, "MoneyH", new { @readonly = "readonly" })
//                                                </div>
//                                            </div>
//                                        </div>
//                                    </div>
//                                </div>
//                            </div>
//                            <div class="row">
//                                <div class="col-sm-5">
//                                    <div class="form-group" id="DivTipoReduccion" style="display: none">
//                                        <label class="col-sm-6 control-label">Reducción: </label>
//                                        <div class="col-sm-6">
//                                            @Html.DropDownListFor(m => m.IdTipoReduccion, Cat.GetList("TipoReduccion"), new { @class = "form-control" })
//                                        </div>
//                                    </div>
//                                </div>
//                                <div class="col-sm-7">
//                                    <div class="form-group">
//                                        <label class="col-sm-3 control-label">Total: </label>
//                                        <div class="col-sm-9">
//                                            @Html.EditorFor(m => m.MontoTotal, "MoneyH", new { @readonly = "readonly" })
//                                        </div>
//                                    </div>
//                                </div>
//                            </div>
//                            <div class="col">
//                                <div class="pull-right">
//                                    <button id = "btnGuardar" style="display:none" type="submit" class="btn btn-primary">
//                                        <i class="fa fa-floppy-o"></i> Guardar
//                                    </button>
//                                </div>
//                            </div>
//                        </fieldset>
//                    </div>


//    [HttpPost]
//public ActionResult Get(string ContratoPasivo)
//{
//    if (String.IsNullOrEmpty(ContratoPasivo))
//    {
//        return PartialMessage("No se estableció la clave de Contrato.", Constants.MessageType.Warning);
//    }

//    ContratoPasivo = ContratoPasivo.Split(new[] { " - " }, StringSplitOptions.None)[0];
//    var itemDb = db.PSV_Contrato.FirstOrDefault(f => f.Contrato == ContratoPasivo && f.IdEstatusContrato == 2);

//    if (itemDb == null)
//    {
//        return PartialMessage("El contrato al que se hace referencia no fue encontrado.", Constants.MessageType.Error);
//    }

//    var SaldoInsoluto = itemDb.PSV_TablaAmortiza.Where(w => !w.Procesado && w.VersionTabla == w.PSV_Contrato.VersionTabla && w.IdTipoTabla == 1)
//        .Sum(s => (decimal?)s.Capital) ?? 0;

//    if (SaldoInsoluto == 0)
//    {
//        return PartialMessage("El contrato Pasivo ya no tiene Vencimientos por procesar, para aplicar el anticipo, no se puede continuar.", Constants.MessageType.Error);
//    }

//    var model = MyApp.Map(itemDb, new InfoGeneralContratoPasivoModel());

//    model.IdTipoReduccion = 1;
//    var hoy = DateTime.Now.Date;

//    model.TablaAmortiza = db.Database.SqlQuery<TablaAmortizaItemDto>(TempQuery.DetalleTablaAmortiza, model.IdContratoPasivo, itemDb.VersionTabla, 1).ToList();

//    model.SaldoVencido = db.PSV_Movimiento.Where(w => w.IdContrato == model.IdContratoPasivo && w.FecMovimiento <= hoy && w.SaldoTotal > 0)
//        .Sum(s => (decimal?)s.SaldoTotal) ?? 0;

//    model.Movimientos = db.Database.SqlQuery<MovimientoItemDto>(TempQuery.DetalleMovimientos, model.IdContratoPasivo).ToList();

//    model.SaldoInsoluto = SaldoInsoluto + model.Movimientos.Where(w => w.EsRenta).Sum(s => (decimal?)s.SaldoCapital) ?? 0;

//    model.Pagos = db.Database.SqlQuery<PagoItemDto>(TempQuery.DetallePagos, model.IdContratoPasivo).ToList();

//    model.FechaAnticipo = DateTime.Now;

//    var ta = model.TablaAmortiza.Where(w => !w.Procesado && w.IdTablaAmortiza != -1).OrderBy(o => o.NoPago).FirstOrDefault();

//    if (ta != null)
//    {
//        model.FechaAnticipo = ta.FecInicial.Value;
//    }

//    return PartialView("_ContratoView", model);
//}