using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, CurrentUserDto>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public LoginUserCommandHandler(SignInManager<AuthUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        LoginRequestDto requestDto = request.Dto;

        AuthUser? user = await _signInManager.UserManager.FindByEmailAsync(requestDto.Email);

        if (user is null)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.InvalidCredentials);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user,
            requestDto.Password,
            isPersistent: true,
            lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            var loginResultDto = new CurrentUserDto(user.Id, user.Email!, user.UserName!);
            return Result.Success(loginResultDto);
        }

        if (signInResult.IsLockedOut)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.UserLockedOut);
        }

        if (signInResult.IsNotAllowed)
        {
            return Result.Failure<CurrentUserDto>(AuthUserErrors.EmailNotConfirmed);
        }

        return Result.Failure<CurrentUserDto>(AuthUserErrors.InvalidCredentials);
    }
}