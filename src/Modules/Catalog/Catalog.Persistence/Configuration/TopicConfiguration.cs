using System;
using System.Collections.Generic;
using System.Text;
using Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Persistence.Configuration
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("Topics");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.DisciplineId)
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasMany(t => t.Questions)
                .WithOne(q => q.Topic)
                .HasForeignKey(q => q.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(t => t.Questions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
