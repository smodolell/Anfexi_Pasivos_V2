namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetPagosByIdContratoQuery : IQuery<Result<List<PagoItemDto>>>
{
    public int IdContrato { get; set; }
}

internal class GetPagosByIdContratoQueryHandler(IDatabaseService databaseService) : IQueryHandler<GetPagosByIdContratoQuery, Result<List<PagoItemDto>>>
{
    private readonly IDatabaseService _databaseService = databaseService;

    public async Task<Result<List<PagoItemDto>>> HandleAsync(GetPagosByIdContratoQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _databaseService.GetDetallePagosAsync(message.IdContrato);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

    }
}


