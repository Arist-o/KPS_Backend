using Microsoft.Extensions.DependencyInjection;
using Shared.Exceptions;

namespace Shared;

/// <summary>
/// Реєстрація спільних Shared-компонентів у DI-контейнері.
/// Викликається в EntryPoint (Program.cs) один раз.
/// Behaviors реєструються окремо через AddMediatR у кожному Application-шарі модуля.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
