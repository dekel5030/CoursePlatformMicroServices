using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommandHandler 
    : ICommandHandler<RefreshAccessTokenCommand, string>
{
    private readonly IWriteDbContext _dbContext;
    private readonly UserManager<AuthUser> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshAccessTokenCommandHandler(
        IWriteDbContext dbContext,
        UserManager<AuthUser> userManager,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<string>> Handle(
        RefreshAccessTokenCommand request,
        CancellationToken cancellationToken = default)
    {
        if (!_tokenService.ValidateRefreshToken(request.RefreshToken))
        {
            return Result.Failure<string>(
                Error.Problem("RefreshToken.Invalid", "Invalid refresh token format"));
        }

        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        // Get all users and find the one with matching refresh token
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
            return Result.Failure<string>(
                Error.NotFound("RefreshToken.NotFound", "Refresh token not found"));
        }

        if (await _userManager.IsLockedOutAsync(authUser))
        {
            return Result.Failure<string>(AuthUserErrors.AccountLocked);
        }

        if (!authUser.EmailConfirmed)
        {
            return Result.Failure<string>(AuthUserErrors.AccountInactive);
        }

        var userClaims = (await _userManager.GetClaimsAsync(authUser)).Select(claim => claim.ToString());
        var roles = await _userManager.GetRolesAsync(authUser);

        var accessToken = _tokenService.GenerateAccessToken(
            new TokenRequestDto { 
                Email = authUser.Email!,
                Roles = roles,
                Permissions = userClaims
        });

        return Result.Success(accessToken);
    }
}
