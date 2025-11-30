using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Application.Extensions;
using Domain.AuthUsers;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, CurrentUserDto>
{
    private readonly SignInManager<AuthUser> _signInManager;
    private readonly UserManager<AuthUser> _userManager;
    private readonly static string _defaultRole = "User";

    public RegisterUserCommandHandler(
        SignInManager<AuthUser> signInManager, 
        UserManager<AuthUser> userManager)
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

        if (!result.Succeeded)
        {
            return result.ToApplicationResult<CurrentUserDto>();
        }

        await _userManager.AddToRoleAsync(user, _defaultRole);

        await _signInManager.SignInAsync(user, true);

        var currentUserDto = new CurrentUserDto(user.Id, user.Email!, user.UserName);
        
        return Result<CurrentUserDto>.Success(currentUserDto);
    }
}