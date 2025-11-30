using Application.Abstractions.Messaging;
using Domain.AuthUsers;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly SignInManager<AuthUser> _signInManager;

    public LogoutCommandHandler(SignInManager<AuthUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();

        return Result.Success();
    }
}
