using Application.Abstractions.Identity;
using Application.Abstractions.Messaging;
using Domain.AuthUsers;
using Kernel;

namespace Application.AuthUsers.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly ISignInManager<AuthUser> _signInManager;

    public LogoutCommandHandler(ISignInManager<AuthUser> signInManager)
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
