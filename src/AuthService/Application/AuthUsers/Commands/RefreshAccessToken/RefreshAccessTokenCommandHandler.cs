using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommandHandler 
    : ICommandHandler<RefreshAccessTokenCommand, string>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;
    private readonly ITokenService _tokenService;

    public RefreshAccessTokenCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
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

        var authUser = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenHash, cancellationToken);

        if (authUser == null)
        {
            return Result.Failure<string>(
                Error.NotFound("RefreshToken.NotFound", "Refresh token not found"));
        }

        if (!authUser.IsRefreshTokenValid(refreshTokenHash))
        {
            return Result.Failure<string>(
                Error.Problem("RefreshToken.Expired", "Refresh token has expired"));
        }

        if (authUser.IsLocked())
        {
            return Result.Failure<string>(AuthUserErrors.AccountLocked);
        }

        if (!authUser.IsActive)
        {
            return Result.Failure<string>(AuthUserErrors.AccountInactive);
        }

        var accessToken = _tokenService.GenerateAccessToken(
            new TokenRequestDto { 
                Email = authUser.Email,
                Roles = authUser.GetRoles().Select(role => role.Name),
                Permissions = authUser.GetPermissions().Select(p => p.Name)
        });

        return Result.Success(accessToken);
    }
}
