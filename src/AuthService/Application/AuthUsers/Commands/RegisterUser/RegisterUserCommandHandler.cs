using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.Roles;
using Kernel;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, CurrentUserDto>
{
    private readonly ISignInManager<AuthUser> _signInManager;
    private readonly IUserManager<AuthUser> _userManager;
    private readonly IRoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Role _defaultRole = null!;

    public RegisterUserCommandHandler(
        ISignInManager<AuthUser> signInManager,
        IUserManager<AuthUser> userManager,
        IUnitOfWork unitOfWork,
        IRoleManager<Role> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
        _defaultRole = _roleManager.Roles.FirstOrDefault(r => r.Name == "User")!;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        RegisterRequestDto requestDto = request.Dto;
        AuthUser user = AuthUser.Create(requestDto.Email, requestDto.UserName, _defaultRole);

        var result = await _userManager.CreateAsync(user, requestDto.Password);

        if (result.IsFailure)
        {
            return Result<CurrentUserDto>.Failure(result.Error);
        }

        // await _signInManager.SignInAsync(user, true);

        var currentUserDto = new CurrentUserDto(user.Id, user.Email!, user.UserName);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CurrentUserDto>.Success(currentUserDto);
    }
}