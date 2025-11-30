using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginResultDto>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public LoginUserCommandHandler(SignInManager<AuthUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result<LoginResultDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        LoginRequestDto requestDto = request.Dto;

        AuthUser? user = await _signInManager.UserManager.FindByEmailAsync(requestDto.Email);

        if (user is null)
        {
            return Result.Failure<LoginResultDto>(AuthUserErrors.InvalidCredentials);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            user,
            requestDto.Password,
            isPersistent: true,
            lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            var loginResultDto = new LoginResultDto(user.Id, user.Email!, user.UserName!);
            return Result.Success(loginResultDto);
        }

        if (signInResult.IsLockedOut)
        {
            return Result.Failure<LoginResultDto>(AuthUserErrors.UserLockedOut);
        }

        if (signInResult.IsNotAllowed)
        {
            return Result.Failure<LoginResultDto>(AuthUserErrors.EmailNotConfirmed);
        }

        return Result.Failure<LoginResultDto>(AuthUserErrors.InvalidCredentials);
    }
}