using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.AuthUsers;
using Kernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IWriteDbContext _dbContext;
    private readonly UserManager<AuthUser> _userManager;
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(
        IWriteDbContext dbContext,
        UserManager<AuthUser> userManager,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        // Find the user with the matching refresh token
        var users = await _dbContext.AuthUsers.ToListAsync(cancellationToken);
        AuthUser? authUser = null;
        
        foreach (var user in users)
        {
            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "AuthService", "refreshToken");
            if (storedToken == refreshTokenHash)
            {
                authUser = user;
                break;
            }
        }

        if (authUser == null)
        {
            return Result.Success();
        }

        // Remove the refresh token
        await _userManager.RemoveAuthenticationTokenAsync(authUser, "AuthService", "refreshToken");

        return Result.Success();
    }
}
