using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Kernel;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, CurrentUserDto>
{
    private readonly ISignService<AuthUser> _signInManager;

    public LoginUserCommandHandler(ISignService<AuthUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result<CurrentUserDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        LoginRequestDto requestDto = request.Dto;


        var loginResultDto = new CurrentUserDto(Guid.Empty, "");
        return Result.Success(loginResultDto);
    }
}