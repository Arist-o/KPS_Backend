using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Common.Models;
using Shared.CQRS;
using Users.Application.DTOs;
using Users.Domain.Entities;

namespace Users.Application.Queries;

public static class GetCurrentUser
{
    public sealed record Query(Guid UserId) : IQuery<Result<UserResponseDto>>;

    public sealed class Handler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
        : IRequestHandler<Query, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result<UserResponseDto>.Failure("User not found.");
            }

            var userDto = mapper.Map<UserResponseDto>(user);
            return Result<UserResponseDto>.Success(userDto);
        }
    }
}
