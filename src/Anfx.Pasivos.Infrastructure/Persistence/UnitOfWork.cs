using Anfx.Pasivos.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Anfx.Pasivos.Infrastructure.Persistence;



public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext = context;
    private IDbContextTransaction? _transaction;
    private readonly IExecutionStrategy _executionStrategy = context.Database.CreateExecutionStrategy();
    private bool _disposed;

    public bool HasActiveTransaction => _transaction != null;

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Ya existe una transacción en curso. " +
                "Puede haber solo una transacción activa por instancia de UnitOfWork.");
        }

        _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay una transacción activa para confirmar.");
        }

        try
        {
            // Primero guardar los cambios del contexto
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Luego confirmar la transacción
            await _transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            // En caso de error, intentar rollback
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log del error de rollback pero no lanzar excepción
                // para no enmascarar la excepción original
                Console.WriteLine($"Error durante rollback: {ex.Message}");
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        // Limpiar el contexto de Entity Framework
        // Esto es importante para evitar estados inconsistentes después de un rollback
        ClearDbContext();
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default)
    {

        // Verificar si ya hay una transacción en curso
        if (HasActiveTransaction)
        {
            // Si ya hay una transacción, ejecutar dentro de ella
            return await operation();
        }

        
        return await _executionStrategy.ExecuteAsync(async () =>
        {

            // Si no hay transacción, crear una nueva
            _transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

            try
            {
                var result = await operation();
                await _transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch (Exception)
            {
                await _transaction.RollbackAsync(cancellationToken);
                ClearDbContext(); // Limpiar el contexto después del rollback
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        });
         
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await operation();
            return true; // Valor dummy para cumplir con la firma
        }, isolationLevel, cancellationToken);
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    private void ClearDbContext()
    {
        // Desconectar todas las entidades del contexto
        // Esto es importante después de un rollback
        var entries = _dbContext.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Detached)
            .ToList();

        foreach (var entry in entries)
        {
            entry.State = EntityState.Detached;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Si hay una transacción activa, hacer rollback antes de desechar
                if (_transaction != null)
                {
                    try
                    {
                        _transaction.Rollback();
                    }
                    catch
                    {
                        // Ignorar errores durante dispose
                    }
                    _transaction.Dispose();
                }

                _dbContext?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Versión async del dispose para usar con using async
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            catch
            {
                // Ignorar errores durante dispose
            }
            await _transaction.DisposeAsync();
        }

        await _dbContext.DisposeAsync();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}