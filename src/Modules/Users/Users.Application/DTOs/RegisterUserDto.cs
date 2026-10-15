using Users.Domain.Enums;

namespace Users.Application.DTOs;

public sealed record RegisterUserDto(string Email, string Password, Role Role);
