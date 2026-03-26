namespace Anfx.Pasivos.Application.Features.Contratos.Queries;

public class GetMovimientosByIdContratoQuery : IQuery<Result<List<MovimientoItemDto>>>
{
    public int IdContrato { get; set; }
}

internal class GetMovimientosByIdContratoQueryHandler(IDatabaseService databaseService) : IQueryHandler<GetMovimientosByIdContratoQuery, Result<List<MovimientoItemDto>>>
{
    private readonly IDatabaseService _databaseService = databaseService;

    public async Task<Result<List<MovimientoItemDto>>> HandleAsync(GetMovimientosByIdContratoQuery message, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _databaseService.GetDetalleMovimientosAsync(message.IdContrato);

            return Result.Success(result);
        }
        catch (Exception ex)
        {

            return Result.Error(ex.Message);
        }

    }
}


