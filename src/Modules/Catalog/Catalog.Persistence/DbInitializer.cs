using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

            await context.Database.MigrateAsync();

            if (!await context.Answers.AnyAsync())
            {
                // Seed initial data
            }

            if (!await context.Disciplines.AnyAsync())
            {
                // Seed initial data for lessons
            }

            if (!await context.Questions.AnyAsync())
            {
                // Seed initial data for tests
            }

            if (!await context.Topics.AnyAsync())
            {
                // Seed initial data for reviews
            }

        }
    }
}
