using Anfx.Pasivos.Application.Common.Interfaces;
using Anfx.Pasivos.Domain.Entities;
using Anfx.Pasivos.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Anfx.Pasivos.Infrastructure.Services;

public class ConsecutivoService : IConsecutivoService
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ConsecutivoService(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtiene el siguiente consecutivo SIN manejar transacciones internamente
    /// La transacción debe ser manejada por el llamador
    /// </summary>
    public async Task<(bool Success, int ConsecutivoGenerado, string ErrorMessage)> ObtenerSiguienteConsecutivoAsync(
        string nombreTabla,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar si estamos dentro de una transacción
            if (!_unitOfWork.HasActiveTransaction)
            {
                return (false, 0,
                    "Este método debe ejecutarse dentro de una transacción Serializable. " +
                    "Use IUnitOfWork.BeginTransactionAsync(IsolationLevel.Serializable) o " +
                    "IUnitOfWork.ExecuteInTransactionAsync()");
            }


            // Usar bloqueo UPDLOCK para asegurar exclusividad
            var lockQuery = @"
                SELECT * FROM Consecutivo WITH (UPDLOCK, ROWLOCK, HOLDLOCK) 
                WHERE NombreTabla = {0}
            ";

            // Bloquear el registro específico
            await _context.Database.ExecuteSqlRawAsync(
                lockQuery,
                //cancellationToken,
                nombreTabla);

            // Buscar el consecutivo
            var consecutivoEntity = await _context.Set<Consecutivo>()
                .FirstOrDefaultAsync(c => c.NombreTabla == nombreTabla, cancellationToken);

            int nuevoConsecutivo;

            if (consecutivoEntity != null)
            {
                // Incrementar consecutivo existente
                consecutivoEntity.IdConsecutivo++;
                consecutivoEntity.FecUltimoCambio = DateTime.Now;
                nuevoConsecutivo = consecutivoEntity.IdConsecutivo;
            }
            else
            {
                // Crear nuevo consecutivo
                consecutivoEntity = new Consecutivo
                {
                    NombreTabla = nombreTabla,
                    IdConsecutivo = 1,
                    FecUltimoCambio = DateTime.Now
                };
                _context.Set<Consecutivo>().Add(consecutivoEntity);
                nuevoConsecutivo = 1;
            }

            // Guardar cambios (la transacción se confirma externamente)
            await _context.SaveChangesAsync(cancellationToken);

            return (true, nuevoConsecutivo, string.Empty);
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
        {
            return HandleSqlException(sqlEx, nombreTabla);
        }
        catch (Exception ex)
        {
            return (false, 0, $"Error al generar consecutivo: {ex.Message}");
        }
    }

    /// <summary>
    /// Método con lógica de reintento para manejar deadlocks
    /// Este método SÍ maneja su propia transacción con reintentos
    /// </summary>
    public async Task<(bool Success, int ConsecutivoGenerado, string ErrorMessage)> ObtenerSiguienteConsecutivoConReintentoAsync(
        string nombreTabla,
        int maxReintentos = 3,
        CancellationToken cancellationToken = default)
    {
        int reintento = 0;

        while (reintento < maxReintentos)
        {
            try
            {
                reintento++;

                // Usar ExecuteInTransactionAsync que maneja transacción automáticamente
                var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // NOTA: Llamamos al método que NO valida la transacción interna
                    return await ObtenerSiguienteConsecutivoSinValidacionAsync(nombreTabla, cancellationToken);
                }, IsolationLevel.Serializable, cancellationToken);

                // Si fue exitoso, retornar
                if (result.Success)
                {
                    return result;
                }

                // Si hubo deadlock, esperar antes de reintentar
                if (result.ErrorMessage.Contains("Deadlock") && reintento < maxReintentos)
                {
                    var delay = TimeSpan.FromMilliseconds(Math.Pow(2, reintento) * 100);
                    await Task.Delay(delay, cancellationToken);
                    continue;
                }

                // Si es otro error, retornar inmediatamente
                return result;
            }
            catch (Exception ex) when (reintento < maxReintentos)
            {
                // Esperar antes de reintentar
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, reintento) * 100);
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                return (false, 0, $"Error después de {maxReintentos} reintentos: {ex.Message}");
            }
        }

        return (false, 0, $"No se pudo generar el consecutivo después de {maxReintentos} reintentos");
    }

    /// <summary>
    /// Versión interna sin validación de transacción (para uso desde ExecuteInTransactionAsync)
    /// </summary>
    private async Task<(bool Success, int ConsecutivoGenerado, string ErrorMessage)> ObtenerSiguienteConsecutivoSinValidacionAsync(
        string nombreTabla,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Usar bloqueo UPDLOCK para asegurar exclusividad
            var lockQuery = @"
                SELECT * FROM cat.Consecutivo WITH (UPDLOCK, ROWLOCK, HOLDLOCK) 
                WHERE NombreTabla = {0}
            ";

            // Bloquear el registro específico
            await _context.Database.ExecuteSqlRawAsync(
                lockQuery,
                cancellationToken,
                nombreTabla);

            // Buscar el consecutivo
            var consecutivoEntity = await _context.Set<Consecutivo>()
                .FirstOrDefaultAsync(c => c.NombreTabla == nombreTabla, cancellationToken);

            int nuevoConsecutivo;

            if (consecutivoEntity != null)
            {
                // Incrementar consecutivo existente
                consecutivoEntity.IdConsecutivo++;
                consecutivoEntity.FecUltimoCambio = DateTime.Now;
                nuevoConsecutivo = consecutivoEntity.IdConsecutivo;
            }
            else
            {
                // Crear nuevo consecutivo
                consecutivoEntity = new Consecutivo
                {
                    NombreTabla = nombreTabla,
                    IdConsecutivo = 1,
                    FecUltimoCambio = DateTime.Now
                };
                _context.Set<Consecutivo>().Add(consecutivoEntity);
                nuevoConsecutivo = 1;
            }

            // Guardar cambios
            await _context.SaveChangesAsync(cancellationToken);

            return (true, nuevoConsecutivo, string.Empty);
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
        {
            return HandleSqlException(sqlEx, nombreTabla);
        }
        catch (Exception ex)
        {
            return (false, 0, $"Error al generar consecutivo: {ex.Message}");
        }
    }

    /// <summary>
    /// Manejo centralizado de excepciones SQL
    /// </summary>
    private (bool Success, int ConsecutivoGenerado, string ErrorMessage) HandleSqlException(
        SqlException sqlEx, string nombreTabla)
    {
        switch (sqlEx.Number)
        {
            case 1205: // Deadlock
                return (false, 0, "Deadlock detectado. La operación se reintentará automáticamente.");

            case 2627: // Violación de clave única
                return (false, 0, $"Ya existe un consecutivo para la tabla '{nombreTabla}'");

            case 1222: // Timeout de bloqueo
                return (false, 0, "Timeout de bloqueo. Por favor reintente la operación.");

            case 1204: // No hay recursos de bloqueo disponibles
                return (false, 0, "No hay recursos de bloqueo disponibles. Intente nuevamente más tarde.");

            case 1213: // Transacción abortada
                return (false, 0, "La transacción fue abortada debido a un error de concurrencia.");

            default:
                return (false, 0, $"Error de base de datos (SQL {sqlEx.Number}): {sqlEx.Message}");
        }
    }

    public async Task<int> ObtenerConsecutivoActualAsync(string nombreTabla, CancellationToken cancellationToken = default)
    {
        // Usar AsNoTracking ya que solo es lectura
        var consecutivo = await _context.Set<Consecutivo>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.NombreTabla == nombreTabla, cancellationToken);

        return consecutivo?.IdConsecutivo ?? 0;
    }

    public async Task<bool> InicializarConsecutivoAsync(string nombreTabla, int valorInicial = 1, CancellationToken cancellationToken = default)
    {
        if (!_unitOfWork.HasActiveTransaction)
        {
            throw new Exception("Este método debe ejecutarse dentro de una transacción Serializable. " +
                "Use IUnitOfWork.BeginTransactionAsync(IsolationLevel.Serializable) o " +
                "IUnitOfWork.ExecuteInTransactionAsync()");
        }
        try
        {

            try
            {
                var existe = await _context.Set<Consecutivo>()
                    .AnyAsync(c => c.NombreTabla == nombreTabla, cancellationToken);

                if (!existe)
                {
                    var consecutivo = new Consecutivo
                    {
                        NombreTabla = nombreTabla,
                        IdConsecutivo = valorInicial,
                        FecUltimoCambio = DateTime.Now
                    };

                    _context.Set<Consecutivo>().Add(consecutivo);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return true;
            }
            catch
            {
                throw;
            }
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            // Si hay violación de clave única, significa que ya existe (concurrencia)
            // En este caso, consideramos que la inicialización fue exitosa
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ReiniciarConsecutivoAsync(string nombreTabla, int nuevoValor = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            
            try
            {
                // Bloquear el registro para evitar concurrencia durante el reinicio
                var lockQuery = @"
                    SELECT * FROM Consecutivo WITH (UPDLOCK, ROWLOCK, HOLDLOCK) 
                    WHERE NombreTabla = {0}
                ";

                await _context.Database.ExecuteSqlRawAsync(
                    lockQuery,
                    nombreTabla);

                var consecutivo = await _context.Set<Consecutivo>()
                    .FirstOrDefaultAsync(c => c.NombreTabla == nombreTabla, cancellationToken);

                if (consecutivo != null)
                {
                    consecutivo.IdConsecutivo = nuevoValor;
                    consecutivo.FecUltimoCambio = DateTime.Now;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    // Si no existe, crear uno nuevo
                    consecutivo = new Consecutivo
                    {
                        NombreTabla = nombreTabla,
                        IdConsecutivo = nuevoValor,
                        FecUltimoCambio = DateTime.Now
                    };
                    _context.Set<Consecutivo>().Add(consecutivo);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return true;
            }
            catch
            {
                throw;
            }
        }
        catch
        {
            return false;
        }
    }

}