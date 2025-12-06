using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, CurrentUserDto>
{
    private readonly ISignInManager<AuthUser> _signInManager;

    public LoginUserCommandHandler(ISignInManager<AuthUser> signInManager)
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

        if (signInResult.IsFailure)
        {
            return Result.Failure<CurrentUserDto>(signInResult.Error);
        }

        var loginResultDto = new CurrentUserDto(user.Id, user.Email!, user.UserName);
        return Result.Success(loginResultDto);
    }
}