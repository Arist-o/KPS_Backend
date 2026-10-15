using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;
using Users.Application.Common.Interfaces;
using Users.Application.Common.Models;
using Users.Application.Services;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MediatR + Pipeline Behaviors (Logging → Validation → Transaction)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // JwtSettings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
