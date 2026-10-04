using Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Persistence.Configuration
{
    public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
    {
        public void Configure(EntityTypeBuilder<Discipline> builder)
        {
            builder.ToTable("Disciplines");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasMany(d => d.Topics)
                .WithOne(t => t.Discipline)
                .HasForeignKey(t => t.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(d => d.Topics)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
