using FluentValidation;
using MediatR;

namespace Shared.Behaviors;

/// <summary>
/// Pipeline Behavior #2 — автоматично знаходить та запускає всіх IValidator&lt;TRequest&gt;.
/// У разі помилок кидає ValidationException → перехоплюється GlobalExceptionHandler → 400 Bad Request.
/// Виконується другим у пайплайні (після LoggingBehavior, перед TransactionBehavior).
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
