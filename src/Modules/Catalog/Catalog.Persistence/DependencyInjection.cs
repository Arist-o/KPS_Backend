using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Application.Common.Interfaces;
using Catalog.Persistence.Context;
using Catalog.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCatalogPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionSqlServer"),
                    sql =>
                    {
                        sql.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );

                        sql.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName);
                    }
                ));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
