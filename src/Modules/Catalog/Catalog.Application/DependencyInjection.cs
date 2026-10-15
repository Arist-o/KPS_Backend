using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;

namespace Catalog.Application;

/// <summary>
/// Реєстрація сервісів шару Catalog.Application.
/// Підключає MediatR з трьома behaviors (Logging → Validation → Transaction),
/// FluentValidation та Mapster.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        // MediatR + Pipeline Behaviors (порядок реєстрації = порядок виконання)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // FluentValidation — автоматична реєстрація всіх Validator-ів з цього Assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Mapster — ручна реєстрація TypeAdapterConfig + IMapper
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(DependencyInjection).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
