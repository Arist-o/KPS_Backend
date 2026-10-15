using Microsoft.EntityFrameworkCore.Storage;
using Shared.Common.Interfaces;
using Users.Application.Common.Interfaces;
using Users.Domain.Entities;
using Users.Persistence.Context;

namespace Users.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly UsersDbContext _context;

    private IDbContextTransaction? _currentTransaction;

    private IRepository<ApplicationUser>? _usersRepository;

    public UnitOfWork(UsersDbContext context)
    {
        _context = context;
    }

    public IRepository<ApplicationUser> Users
        => _usersRepository ??= new Repository<ApplicationUser>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException(
                "Transaction is already active. Nested transactions are not supported.");

        _currentTransaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("Немає активної транзакції для коміту.");

        try
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) return;

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public IExecutionStrategy CreateExecutionStrategy()
        => _context.Database.CreateExecutionStrategy();

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
            await _currentTransaction.DisposeAsync();

        await _context.DisposeAsync();
    }
}
