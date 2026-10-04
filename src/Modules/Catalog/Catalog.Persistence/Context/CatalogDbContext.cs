using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Persistence.Context
{
    public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
    {
        public DbSet<Answer> Answers => Set<Answer>();

        public DbSet<Discipline> Disciplines => Set<Discipline>();

        public DbSet<Question> Questions => Set<Question>();

        public DbSet<Topic> Topics => Set<Topic>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Answer answer
                    && entry.State == EntityState.Modified)
                {
                    answer.UpdateTimeStamp();
                }

                if (entry.Entity is Discipline discipline
                    && entry.State == EntityState.Modified)
                {
                    discipline.UpdateTimeStamp();
                }

                if (entry.Entity is Question question
                    && entry.State == EntityState.Modified)
                {
                    question.UpdateTimeStamp();
                }

                if (entry.Entity is Topic topic
                    && entry.State == EntityState.Modified)
                {
                    topic.UpdateTimeStamp();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
