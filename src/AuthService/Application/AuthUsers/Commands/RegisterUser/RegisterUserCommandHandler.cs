using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Kernel;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, CurrentUserDto>
{
    private readonly ISignInManager<AuthUser> _signInManager;
    private readonly IUserManager<AuthUser> _userManager;
    private const string DefaultRole = "User";

    public RegisterUserCommandHandler(
        ISignInManager<AuthUser> signInManager,
        IUserManager<AuthUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        RegisterRequestDto requestDto = request.Dto;
        AuthUser user = AuthUser.Create(requestDto.Email, requestDto.UserName);

        var result = await _userManager.CreateAsync(user, requestDto.Password);

        if (result.IsFailure)
        {
            return Result<CurrentUserDto>.Failure(result.Error);
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, DefaultRole);
        if (addToRoleResult.IsFailure)
        {
            return Result<CurrentUserDto>.Failure(addToRoleResult.Error);
        }

        await _signInManager.SignInAsync(user, true);

        var currentUserDto = new CurrentUserDto(user.Id, user.Email!, user.UserName);

        return Result<CurrentUserDto>.Success(currentUserDto);
    }
}