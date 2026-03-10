using Anfx.Pasivos.Application.Common.DTOs;
using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Application.Features.Contratos.DTOs;
using Anfx.Pasivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Anfx.Pasivos.Infrastructure.Services;

internal class DatabaseService : IDatabaseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseService> _logger;

    // Constantes SQL
    private const string DetalleMovimientosSql = @"
SELECT pm.IdMovimiento,
       pm.Descripcion,
       pm.NoPago,
       pm.FecMovimiento,
       pm.Capital,
       pm.Interes,
       pm.IVA,
       pm.Total,
       pm.SaldoCapital,
       pm.SaldoInteres,
       pm.SaldoIVA,
       pm.SaldoTotal,
       CAST(
           CASE 
                WHEN (ptc.IdTipoMovimiento = pm.IdTipoMovimiento) THEN 1
                ELSE 0
           END AS BIT
       ) EsRenta
FROM   PSV_Movimiento pm
       INNER JOIN PSV_Contrato pc
            ON  pc.IdContrato = pm.IdContrato
       INNER JOIN PSV_TipoCredito ptc
            ON  ptc.IdTipoCredito = pc.IdTipoCredito
WHERE  pm.IdContrato = {0}
 UNION ALL
SELECT -1                                  IdMovimiento,
       ''                                  Descripcion,
       0                                   NoPago,
       CAST(NULL AS DATE)                  FecMovimiento,
       ISNULL(SUM(pm.Capital), 0)          Capital,
       ISNULL(SUM(pm.Interes), 0)          Interes,
       ISNULL(SUM(pm.IVA), 0)              IVA,
       ISNULL(SUM(pm.Total), 0)            Total,
       ISNULL(SUM(pm.SaldoCapital), 0)     SaldoCapital,
       ISNULL(SUM(pm.SaldoInteres), 0)     SaldoInteres,
       ISNULL(SUM(pm.SaldoIVA), 0)         SaldoIVA,
       ISNULL(SUM(pm.SaldoTotal), 0)       SaldoTotal,
       CAST(0 AS BIT)                      EsRenta
FROM   PSV_Movimiento                      pm
WHERE  pm.IdContrato = {0}";

    private const string DetallePagosSql = @"
        DECLARE @tResult TABLE (
            IdPago INT,
            IdContratoPasivo INT,
            TipoPago VARCHAR(200),
            CuentaBancaria VARCHAR(200),
            FecPagoValor DATETIME,
            FecPagoRegistro DATETIME,
            MontoPago DECIMAL(13,2),
            MontoAplicado DECIMAL(13,2),
            MontoAplicadoOtros DECIMAL(13,2),
            SaldoPago DECIMAL(13,2)
        );

        INSERT INTO @tResult
        SELECT
            pp.IdPago,
            pm.IdContrato IdContratoPasivo,
            ptp.TipoPago,
            pcb.CuentaBancaria,
            pp.FecPagoValor,
            pp.FecPagoRegistro,
            pp.MontoPago,
            SUM(prpm.TotalPagado) [MontoAplicado],
            CAST(0 AS DECIMAL(13,2)) [MontoAplicadoOtros],
            pp.SaldoPago
        FROM PSV_Pago pp
        INNER JOIN PSV_TipoPago ptp ON ptp.IdTipoPago = pp.IdTipoPago
        INNER JOIN PSV_CuentaBancaria pcb ON pcb.IdCuentaBancaria = pp.IdCuentaBancaria
        INNER JOIN PSV_RelPagoMovimiento prpm ON prpm.IdPago = pp.IdPago AND prpm.Estatus = 1
        INNER JOIN PSV_Movimiento pm ON pm.IdMovimiento = prpm.IdMovimiento
        WHERE pm.IdContrato = {0}
        GROUP BY
            pp.IdPago,
            ptp.TipoPago,
            pcb.CuentaBancaria,
            pp.FecPagoValor,
            pp.FecPagoRegistro,
            pm.IdContrato,
            pp.MontoPago,
            pp.SaldoPago

        INSERT INTO @tResult
        SELECT
            pp.IdPago,
            pc.IdContrato IdContratoPasivo,
            ptp.TipoPago,
            pcb.CuentaBancaria,
            pp.FecPagoValor,
            pp.FecPagoRegistro,
            pp.MontoPago,
            ISNULL(SUM(prpm.TotalPagado), 0) [MontoAplicado],
            CAST(0 AS DECIMAL(13,2)) [MontoAplicadoOtros],
            pp.SaldoPago
        FROM PSV_Pago pp
        INNER JOIN PSV_TipoPago ptp ON ptp.IdTipoPago = pp.IdTipoPago
        INNER JOIN PSV_Contrato pc ON pc.Contrato = pp.Contrato AND pc.IdFondeador = pp.IdFondeador
        INNER JOIN PSV_CuentaBancaria pcb ON pcb.IdCuentaBancaria = pp.IdCuentaBancaria
        LEFT  JOIN PSV_RelPagoMovimiento prpm ON prpm.IdPago = pp.IdPago AND prpm.Estatus = 1
        LEFT  JOIN PSV_Movimiento pm ON pm.IdMovimiento = prpm.IdMovimiento AND pm.IdContrato = {0}
        WHERE pc.IdContrato = {0}
        AND prpm.IdPagoMovimiento IS NULL
        GROUP BY
            pp.IdPago,
            ptp.TipoPago,
            pcb.CuentaBancaria,
            pp.FecPagoValor,
            pp.FecPagoRegistro,
            pc.IdContrato,
            pp.MontoPago,
            pp.SaldoPago;

        UPDATE t
        SET t.MontoAplicadoOtros = tt.MontoAplicadoOtros
        FROM @tResult t
        INNER JOIN (
            SELECT t.IdPago, SUM(prpm.TotalPagado) MontoAplicadoOtros 
            FROM PSV_RelPagoMovimiento prpm
            INNER JOIN PSV_Movimiento pm ON pm.IdMovimiento = prpm.IdMovimiento
            INNER JOIN @tResult t ON t.IdPago = prpm.IdPago AND pm.IdContrato <> t.IdContratoPasivo
            WHERE prpm.Estatus = 1
            GROUP BY t.IdPago
        ) tt ON tt.IdPago = t.IdPago;

        SELECT 
            t.IdPago,
            t.IdContratoPasivo,
            t.TipoPago,
            t.CuentaBancaria,
            t.FecPagoValor,
            t.FecPagoRegistro,
            t.MontoPago,
            t.MontoAplicado,
            t.MontoAplicadoOtros,
            t.SaldoPago
        FROM   @tResult t
        UNION ALL
        SELECT
            -1 IdPago,
            0 IdContratoPasivo,
            '' TipoPago,
            '' CuentaBancaria,
            CAST(NULL AS DATETIME) FecPagoValor,
            CAST(NULL AS DATETIME) FecPagoRegistro,
            SUM(tt.MontoPago) MontoPago,
            SUM(tt.MontoAplicado) MontoAplicado,
            SUM(tt.MontoAplicadoOtros) MontoAplicadoOtros,
            SUM(tt.SaldoPago) SaldoPago
        FROM @tResult tt
        GROUP BY tt.IdContratoPasivo";

    private const string DetalleTablaAmortizaSql = @"
        SELECT
            ta.IdTablaAmortiza,
            ta.NoPago,
            ta.FecInicial,
            ta.FecFinal,
            ta.FecVencimiento,
            ta.SaldoInicial,
            ta.Capital,
            ta.Interes,
            ta.IVA,
            ta.Seguro,
            ta.Total,
            ta.Procesado 
        FROM PSV_TablaAmortiza ta
        WHERE ta.IdContrato = {0}
        AND ta.VersionTabla = {1}
        AND ta.IdTipoTabla = {2}
        UNION ALL
        SELECT
            -1 IdTablaAmortiza,
            0 NoPago,
            CAST(NULL AS DATE) FecInicial,
            CAST(NULL AS DATE) FecFinal,
            CAST(NULL AS DATE) FecVencimiento,
            CAST(0 AS DECIMAL(13, 2)) SaldoInicial,
            SUM(ta.Capital) Capital,
            SUM(ta.Interes) Interes,
            SUM(ta.IVA) IVA,
            SUM(ta.Seguro) Seguro,
            SUM(ta.Total) Total,
            CAST(0 AS BIT) Procesado 
        FROM PSV_TablaAmortiza ta
        WHERE ta.IdContrato = {0}
        AND ta.VersionTabla = {1}
        AND ta.IdTipoTabla = {2}
        GROUP BY ta.IdContrato, ta.VersionTabla, ta.IdTipoTabla";

    public DatabaseService(
        ApplicationDbContext context,
        ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el detalle de movimientos de un contrato (DetalleMovimientos)
    /// </summary>
    public async Task<List<MovimientoItemDto>> GetDetalleMovimientosAsync(
        int idContratoPasivo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Obteniendo detalle de movimientos para contrato {IdContrato}", idContratoPasivo);

            // Usar FromSqlRaw con parámetros posicionales
            var resultados = await _context.Database.SqlQueryRaw<MovimientoItemDto>(
                DetalleMovimientosSql,
                idContratoPasivo
            ).ToListAsync(cancellationToken);

            return resultados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo detalle de movimientos para contrato {IdContrato}", idContratoPasivo);
            throw;
        }
    }

    /// <summary>
    /// Obtiene el detalle de pagos de un contrato (DetallePagos)
    /// </summary>
    public async Task<List<PagoItemDto>> GetDetallePagosAsync(
        int idContratoPasivo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Obteniendo detalle de pagos para contrato {IdContrato}", idContratoPasivo);

            // Usar FromSqlRaw con parámetros posicionales
            var resultados = await _context.Database.SqlQueryRaw<PagoItemDto>(
                DetallePagosSql,
                idContratoPasivo
            ).ToListAsync(cancellationToken);

            return resultados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo detalle de pagos para contrato {IdContrato}", idContratoPasivo);
            throw;
        }
    }

    /// <summary>
    /// Obtiene el detalle de tabla de amortización de un contrato (DetalleTablaAmortiza)
    /// </summary>
    public async Task<List<TablaAmortizaItemDto>> GetDetalleTablaAmortizaAsync(
        int idContratoPasivo,
        int versionTabla,
        int idTipoTabla = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Obteniendo detalle de tabla de amortización para contrato {IdContrato}, versión {Version}",
                idContratoPasivo, versionTabla);

            // Usar FromSqlRaw con parámetros posicionales
            var resultados = await _context.Database.SqlQueryRaw<TablaAmortizaItemDto>(
                DetalleTablaAmortizaSql,
                idContratoPasivo,
                versionTabla,
                idTipoTabla
            ).ToListAsync(cancellationToken);

            return resultados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo detalle de tabla de amortización para contrato {IdContrato}", idContratoPasivo);
            throw;
        }
    }

   
}