using Anfx.Pasivos.Application.Common.Models.StoredProcedures;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetInteresQuery : IQuery<Result<decimal>>
{
    public int IdContrato { get; set; }
    public DateOnly FechaCorte { get; set; }
    public decimal MontoAnticipo { get; set; }

}


internal class GetInteresQueryHandler(IApplicationDbContext context) : IQueryHandler<GetInteresQuery, Result<decimal>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<decimal>> HandleAsync(GetInteresQuery message, CancellationToken cancellationToken = default)
    {
        var resultSp = await _context.Procedures.usp_PSV_CalculaInteresAsync(
            message.IdContrato,
            message.FechaCorte,
            message.MontoAnticipo
            );
        var result = resultSp.FirstOrDefault();
        if (result == null) return Result.Error("Error en Calculo");
        return Result.Success(result.MontoInteres ?? 0);
    }
}
//[HttpGet]
//public ActionResult CalculaInteres(int id, string fc, decimal ma)
//{
//    try
//    {
//        var FechaCorte = DateTime.ParseExact(fc, "dd/MM/yyyy", new CultureInfo("es-MX"));
//        var usp = new usp_PSV_CalculaInteres { IdContrato = id, FechaCorte = FechaCorte, MontoAnticipo = ma };
//        var result = db.Database.ExecuteStoredProcedure<usp_PSV_CalculaInteres_Result>(usp).FirstOrDefault();
//        if (result != null)
//        {
//            if (string.IsNullOrEmpty(result.Error))
//            {
//                return Json(new { Error = false, MontoInteres = result.MontoInteres }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Error = true, ErrorMessage = result.Error }, JsonRequestBehavior.AllowGet);
//        }
//        return Json(new { Error = true, ErrorMessage = "El procedimiento no devolvió resultados." }, JsonRequestBehavior.AllowGet);
//    }
//    catch (Exception ex)
//    {
//        return Json(new { Error = true, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
//    }
//}
//internal class Class1
//{
//}
