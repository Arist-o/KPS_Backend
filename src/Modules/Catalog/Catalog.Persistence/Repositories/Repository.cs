using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Catalog.Application.Common.Interfaces;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
    internal sealed class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly CatalogDbContext _context;

        private readonly DbSet<TEntity> _dbSet;

        public Repository(CatalogDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync([id], cancellationToken);

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

        public async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default
            )
        => await _dbSet.AnyAsync(predicate, cancellationToken);

        public IQueryable<TEntity> Query(bool asNoTracking = true)
            => asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public void Remove(TEntity entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    }
}
