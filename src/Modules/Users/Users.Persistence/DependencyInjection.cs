using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Common.Interfaces;
using Users.Domain.Entities;
using Users.Persistence.Context;
using Users.Persistence.Repositories;

namespace Users.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnectionSqlServer"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sql.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName);

                    // Ізольована таблиця міграцій для схеми identity
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
                }));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHttpContextAccessor();

        return services;
    }
}
