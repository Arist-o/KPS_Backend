namespace Users.Application.DTOs;

public sealed record AuthResponseDto(string Token, UserResponseDto User);
