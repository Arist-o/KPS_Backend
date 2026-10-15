using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Persistence.Context;

public class UsersDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .IsRequired();
        });
    }
}
