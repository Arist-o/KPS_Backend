using Microsoft.EntityFrameworkCore.Storage;

namespace Shared.Common.Interfaces;

/// <summary>
/// Базовий контракт Unit of Work — без прив'язки до конкретних сутностей.
/// Кожен модульний IUnitOfWork успадковується від цього інтерфейсу і додає
/// власні репозиторії (наприклад, IRepository&lt;Discipline&gt; Disciplines).
/// </summary>
public interface IBaseUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    IExecutionStrategy CreateExecutionStrategy();
}
