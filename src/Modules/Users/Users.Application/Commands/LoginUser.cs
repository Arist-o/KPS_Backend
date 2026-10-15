using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Common.Models;
using Shared.CQRS;
using Users.Application.Common.Interfaces;
using Users.Application.DTOs;
using Users.Domain.Entities;

namespace Users.Application.Commands;

public static class LoginUser
{
    public sealed record Command(LoginUserDto Dto) : ICommand<Result<AuthResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Password cannot be empty.");
        }
    }

    public sealed class Handler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
        : IRequestHandler<Command, Result<AuthResponseDto>>
    {
        public async Task<Result<AuthResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Dto.Email);
            if (user is null)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            var passwordValid = await signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, lockoutOnFailure: false);
            if (!passwordValid.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("Invalid email or password.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtTokenGenerator.GenerateToken(user, roles);
            var userDto = mapper.Map<UserResponseDto>(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto(token, userDto));
        }
    }
}
