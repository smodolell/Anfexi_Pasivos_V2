namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetGetTablaAmortizaQuery : IQuery<Result<List<TablaAmortizaItemDto>>>
{
    public int IdContrato { get; set; }
    public int IdTipoTabla { get; set; }

}
internal class GetGetTablaAmortizaQueryHandler : IQueryHandler<GetGetTablaAmortizaQuery, Result<List<TablaAmortizaItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDatabaseService _databaseService;
    public GetGetTablaAmortizaQueryHandler(IApplicationDbContext context, IDatabaseService databaseService)
    {
        _context = context;
        _databaseService = databaseService;
    }
    public async Task<Result<List<TablaAmortizaItemDto>>> HandleAsync(GetGetTablaAmortizaQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var contrato = await _context.PSV_Contrato.SingleOrDefaultAsync(r => r.IdContrato == request.IdContrato);
            if (contrato == null)
            {
                return Result.NotFound("Contrato no encontrado");
            }

            var result = await _databaseService.GetDetalleTablaAmortizaAsync(request.IdContrato, contrato.VersionTabla ?? 1, request.IdTipoTabla);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }
}