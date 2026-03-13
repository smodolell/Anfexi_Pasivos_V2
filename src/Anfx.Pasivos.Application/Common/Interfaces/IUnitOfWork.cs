using System.Data;

namespace Anfx.Pasivos.Application.Common.Interfaces;


public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia una transacción con nivel de aislamiento por defecto
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inicia una transacción con nivel de aislamiento específico
    /// </summary>
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ejecuta una operación dentro de una transacción con manejo automático
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ejecuta una operación dentro de una transacción con manejo automático (sin retorno)
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si hay una transacción activa
    /// </summary>
    bool HasActiveTransaction { get; }
}