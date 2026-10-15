using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Common.Models;
using Shared.CQRS;
using Users.Application.DTOs;
using Users.Domain.Entities;

namespace Users.Application.Commands;

public static class RegisterUser
{
    public sealed record Command(RegisterUserDto Dto) : ICommand<Result<UserResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Dto.Role)
                .IsInEnum().WithMessage("Invalid role.");
        }
    }

    public sealed class Handler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IMapper mapper)
        : IRequestHandler<Command, Result<UserResponseDto>>
    {
        public async Task<Result<UserResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Dto.Email);
            if (existingUser is not null)
            {
                return Result<UserResponseDto>.Failure("User with this email already exists.");
            }

            var user = new ApplicationUser(request.Dto.Email, request.Dto.Email, request.Dto.Role);

            var createResult = await userManager.CreateAsync(user, request.Dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return Result<UserResponseDto>.Failure(errors);
            }

            var roleName = request.Dto.Role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleCreateResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                if (!roleCreateResult.Succeeded)
                {
                    var errors = string.Join("; ", roleCreateResult.Errors.Select(e => e.Description));
                    return Result<UserResponseDto>.Failure($"Failed to create role: {errors}");
                }
            }

            var addToRoleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addToRoleResult.Errors.Select(e => e.Description));
                return Result<UserResponseDto>.Failure($"Failed to add user to role: {errors}");
            }

            var responseDto = mapper.Map<UserResponseDto>(user);
            return Result<UserResponseDto>.Success(responseDto);
        }
    }
}
