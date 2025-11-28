using Application.Abstractions.Messaging;
using Domain.AuthUsers;
using Kernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly UserManager<AuthUser> _userManager;

    public LogoutCommandHandler(UserManager<AuthUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken = default)
    {
        var authUser = await _userManager.Users
            .FirstOrDefaultAsync(user => user.Email == request.Email, cancellationToken);

        if (authUser == null)
        {
            return Result.Success();
        }

        await _userManager.RemoveAuthenticationTokenAsync(authUser, "AuthService", "RefreshToken");

        return Result.Success();
    }
}
