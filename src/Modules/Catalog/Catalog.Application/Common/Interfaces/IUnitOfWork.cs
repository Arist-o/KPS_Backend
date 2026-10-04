using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.Application.Common.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<Answer> Answers { get; }

        IRepository<Discipline> Disciplines { get; }

        IRepository<Question> Questions { get; }

        IRepository<Topic> Topics { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        IExecutionStrategy CreateExecutionStrategy();
    }
}
