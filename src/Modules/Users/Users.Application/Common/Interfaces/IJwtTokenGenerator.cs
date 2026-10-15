using Users.Domain.Entities;

namespace Users.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}
