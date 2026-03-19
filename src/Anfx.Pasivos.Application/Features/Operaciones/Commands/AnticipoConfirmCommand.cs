namespace Anfx.Pasivos.Application.Features.Operaciones.Commands;

public class AnticipoConfirmCommand : ICommand<Result>
{
    public int IdContrato { get; set; }
    public int IdTipoTerminacion { get; set; }
    public int IdTipoReduccion { get; set; }
    public DateTime FechaAnticipo { get; set; }
    public decimal MontoAnticipo { get; set; }
    public decimal MontoInteres { get; set; }
    public decimal MontoPena { get; set; }
    public decimal MontoIVA_Interes { get; set; }
    public decimal MontoIVA_Pena { get; set; }
    public decimal MontoTotal { get; set; }
    public bool EsLiquidacion { get; set; }

}


internal class AnticipoConfirmCommandHandler(IApplicationDbContext context, IMapper mapper) : ICommandHandler<AnticipoConfirmCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> HandleAsync(AnticipoConfirmCommand message, CancellationToken cancellationToken = default)
    {
        if (message.IdTipoReduccion != 1) return Result.Error("No Implementado");

        var itemDb = _mapper.Map<PSV_Terminacion>(message);
        itemDb.FechaRegistro = DateTime.Now;

        _context.PSV_Terminacion.Add(itemDb);
        await _context.SaveChangesAsync(cancellationToken);

        if (message.EsLiquidacion)
        {
            var resultSp = await _context.Procedures.usp_PSV_LiquidacionAsync(itemDb.IdTerminacion);
            var result = resultSp.FirstOrDefault();
            if (result != null && string.IsNullOrEmpty(result.Error))
            {
                return Result.SuccessWithMessage("Liquidacion aplicada correctamente.");
            }
        }
        else
        {
            var contrato = await _context.PSV_Contrato
                .SingleOrDefaultAsync(f => f.IdContrato == message.IdContrato);
            var resultSp = await _context.Procedures.usp_PSV_AplicaAnticipo_CIAsync(itemDb.IdTerminacion);
            var result = resultSp.FirstOrDefault();
            if (result != null && string.IsNullOrEmpty(result.Error))
            {
                return Result.SuccessWithMessage("Anticipo a Capital Capitalizable aplicado correctamente.");
            }


        }
        return Result.Error("Error al Aplicar una terminacion");
    }
}

//    [HttpPost]
//public ActionResult Save(AnticipoCapitalPasivoModel model)
//{
//    var ErrorMessage = "Formulario incompleto o no válido";
//    if (ModelState.IsValid)
//    {
//        using (var ts = new TransactionScope())
//        {
//            try
//            {
//                var itemDb = MyApp.Map(model, new Data.PSV_Terminacion { FechaRegistro = DateTime.Now });
//                db.PSV_Terminacion.Add(itemDb);
//                db.SaveChanges();

//                if (model.EsLiquidacion)
//                {
//                    var result = db.Database.ExecuteStoredProcedure<string>(new usp_PSV_Liquidacion(itemDb.IdTerminacion)).FirstOrDefault();
//                    if (string.IsNullOrEmpty(result))
//                    {
//                        ts.Complete();
//                        return Aplicado("Liquidacion aplicada correctamente.");
//                    }
//                    ErrorMessage = result;
//                }
//                else
//                {
//                    var contrato = db.PSV_Contrato.First(f => f.IdContrato == model.IdContratoPasivo);
//                    //switch (contrato.PSV_TipoTablaAmortiza.EsCapitalizable)
//                    switch (false)
//                    {
//                        case true:
//                            {
//                                var result = db.Database.ExecuteStoredProcedure<string>(new usp_PSV_AplicaAnticipo_CI(itemDb.IdTerminacion)).FirstOrDefault();
//                                if (string.IsNullOrEmpty(result))
//                                {
//                                    ts.Complete();
//                                    ts.Dispose();
//                                    return Aplicado("Anticipo a Capital Capitalizable aplicado correctamente.");
//                                }
//                                ErrorMessage = result;
//                            }
//                            break;
//                        default:
//                            switch (model.IdTipoReduccion)
//                            {
//                                case 1:
//                                    {
//                                        var result = db.Database.ExecuteStoredProcedure<string>(new usp_PSV_AnticipoACapital(itemDb.IdTerminacion)).FirstOrDefault();
//                                        if (string.IsNullOrEmpty(result))
//                                        {
//                                            ts.Complete();
//                                            ts.Dispose();
//                                            return Aplicado("Anticipo a Capital aplicado correctamente.");
//                                        }
//                                        ErrorMessage = result;
//                                    }
//                                    break;
//                                case 2:
//                                    {
//                                        var result = db.Database.ExecuteStoredProcedure<string>(new usp_PSV_AnticipoAPlazo(itemDb.IdTerminacion)).FirstOrDefault();
//                                        if (string.IsNullOrEmpty(result))
//                                        {
//                                            ts.Complete();
//                                            ts.Dispose();
//                                            return Aplicado("Anticipo a Plazo aplicado correctamente.");
//                                        }
//                                        ErrorMessage = result;

//                                    }
//                                    break;
//                            }
//                            break;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ts.Dispose();
//                ErrorMessage = ex.Message;
//            }
//        }
//    }

//    return PartialMessage(ErrorMessage, Constants.MessageType.Error);
//}
