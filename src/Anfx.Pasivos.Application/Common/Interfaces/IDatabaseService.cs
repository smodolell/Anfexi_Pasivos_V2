namespace Anfx.Pasivos.Application.Common.Interfaces;

public interface IDatabaseService
{
    Task<List<TablaAmortizaItemDto>> GetDetalleTablaAmortizaAsync(
        int idContratoPasivo,
        int versionTabla,
        int idTipoTabla = 1,
        CancellationToken cancellationToken = default
    );

    Task<List<PagoItemDto>> GetDetallePagosAsync(
        int idContratoPasivo,
        CancellationToken cancellationToken = default
    );


    Task<List<MovimientoItemDto>> GetDetalleMovimientosAsync(
        int idContratoPasivo,
        CancellationToken cancellationToken = default
    );

}
