using Anfx.Pasivos.Application.Features.Contratos.DTOs;

namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetPagosDetalleQuery:IQuery<Result<PagoDetalleDto>>
{
    public int IdPago { get; set; }


}

internal class GetPagosDetalleQueryHandler(IApplicationDbContext context,IDatabaseService databaseService) : IQueryHandler<GetPagosDetalleQuery, Result<PagoDetalleDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDatabaseService _databaseService = databaseService;

    public async Task<Result<PagoDetalleDto>> HandleAsync(GetPagosDetalleQuery message, CancellationToken cancellationToken = default)
    {
        var pago = await _context.PSV_Pago.SingleOrDefaultAsync(r => r.IdPago == message.IdPago);

        if (pago == null) return Result.NotFound("Pago no encontrado");

        var result = new PagoDetalleDto
        {
            IdPago = pago.IdPago,
            Detalle = await _databaseService.GetDetalleAplicacionDePagoAsync(pago.IdPago)
        };

        return Result.Success(result);
    }
}
