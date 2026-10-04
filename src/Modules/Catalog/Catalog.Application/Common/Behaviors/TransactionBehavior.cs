using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Common.Behaviors
{
    public sealed class TransactionBehavior<TRequest, TResponse>(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not IBaseCommand)
                return await next();

            var requestName = typeof(TRequest).Name;
            var strategy = unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                logger.LogInformation("[Tx] BEGIN — {Request}", requestName);

                try
                {
                    var response = await next();

                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    await unitOfWork.CommitTransactionAsync(cancellationToken);

                    logger.LogInformation("[Tx] COMMIT — {Request}", requestName);
                    return response;
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    logger.LogError(ex, "[Tx] ROLLBACK — {Request}: {Message}", requestName, ex.Message);
                    throw;
                }
            });
        }
    }

    public interface IBaseCommand;

    public interface ICommand : IRequest, IBaseCommand;

    public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand;

    public interface IQuery<out TResponse> : IRequest<TResponse>;
}
