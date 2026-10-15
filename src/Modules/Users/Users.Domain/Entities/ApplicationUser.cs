using Microsoft.AspNetCore.Identity;
using Users.Domain.Enums;

namespace Users.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public Role Role { get; set; } = Role.Player;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        SecurityStamp = Guid.NewGuid().ToString();
    }

    public ApplicationUser(string userName) : this()
    {
        UserName = userName;
    }

    public ApplicationUser(string userName, string email, Role role = Role.Player) : this()
    {
        UserName = userName;
        Email = email;
        Role = role;
    }
}
