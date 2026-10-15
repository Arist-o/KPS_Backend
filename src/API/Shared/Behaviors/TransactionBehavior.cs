using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common.Interfaces;
using Shared.CQRS;

namespace Shared.Behaviors;

/// <summary>
/// Pipeline Behavior #3 — обгортає команди (IBaseCommand) у БД-транзакцію.
/// Запити (IQuery) пропускаються без транзакції.
/// Виконується третім у пайплайні (після ValidationBehavior).
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse>(
    IBaseUnitOfWork unitOfWork,
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

        // ExecutionStrategy.ExecuteInTransactionAsync — розширення EF Core для зручної роботи з транзакціями
        return await strategy.ExecuteInTransactionAsync(
            operation: async ct =>
            {
                logger.LogInformation("[Tx] BEGIN — {Request}", requestName);

                var response = await next();

                await unitOfWork.SaveChangesAsync(ct);

                logger.LogInformation("[Tx] COMMIT — {Request}", requestName);
                return response;
            },
            verifySucceeded: _ => Task.FromResult(false),
            cancellationToken: cancellationToken);
    }
}
