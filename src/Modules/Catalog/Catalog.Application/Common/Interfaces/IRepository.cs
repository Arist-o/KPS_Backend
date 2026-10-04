using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Catalog.Application.Common.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default
        );

        Task<IReadOnlyList<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default
        );

        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default
        );

        Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default
        );

        IQueryable<TEntity> Query(bool asNoTracking = true);

        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default
        );

        void Update(TEntity entity);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
