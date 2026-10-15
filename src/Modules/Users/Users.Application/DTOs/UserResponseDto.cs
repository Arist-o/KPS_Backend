namespace Users.Application.DTOs;

public sealed record UserResponseDto(Guid Id, string Email, string Role, DateTime CreatedAt);
