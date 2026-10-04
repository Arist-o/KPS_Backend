using Catalog.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Entity;
using Catalog.Persistence.Context;

namespace Catalog.Persistence.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogDbContext _context;

        private IDbContextTransaction? _currentTransaction;

        private IRepository<Answer>? _answersRepository;

        private IRepository<Discipline>? _disciplinesRepository;

        private IRepository<Question>? _questionsRepository;

        private IRepository<Topic>? _topicsRepository;

        public UnitOfWork(CatalogDbContext context)
        {
            _context = context;
        }

        public IRepository<Answer> Answers
            => _answersRepository ??= new Repository<Answer>(_context);

        public IRepository<Discipline> Disciplines
            => _disciplinesRepository ??= new Repository<Discipline>(_context);

        public IRepository<Question> Questions
            => _questionsRepository ??= new Repository<Question>(_context);

        public IRepository<Topic> Topics
            => _topicsRepository ??= new Repository<Topic>(_context);

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
                throw new InvalidOperationException(
                    "Немає активної транзакції для коміту.");

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
}
