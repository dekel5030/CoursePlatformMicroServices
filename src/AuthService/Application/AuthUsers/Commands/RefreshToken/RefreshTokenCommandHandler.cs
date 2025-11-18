using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokensDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken = default)
    {
        // Validate refresh token format
        if (!_tokenService.ValidateRefreshToken(request.RefreshToken))
        {
            return Result.Failure<AuthTokensDto>(
                Error.Problem("RefreshToken.Invalid", "Invalid refresh token format"));
        }

        // Hash the provided refresh token to compare with stored hash
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        // Find user by hashed refresh token
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
            return Result.Failure<AuthTokensDto>(
                Error.NotFound("RefreshToken.NotFound", "Refresh token not found"));
        }

        // Validate refresh token expiration
        if (!authUser.IsRefreshTokenValid(refreshTokenHash))
        {
            return Result.Failure<AuthTokensDto>(
                Error.Problem("RefreshToken.Expired", "Refresh token has expired"));
        }

        // Check if account is locked
        if (authUser.IsLocked())
        {
            return Result.Failure<AuthTokensDto>(AuthUserErrors.AccountLocked);
        }

        // Check if account is active
        if (!authUser.IsActive)
        {
            return Result.Failure<AuthTokensDto>(AuthUserErrors.AccountInactive);
        }

        // Generate new refresh token (token rotation)
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // 7 days expiration
        
        // Hash the new refresh token before storing
        var newRefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);
        authUser.SetRefreshToken(newRefreshTokenHash, refreshTokenExpiresAt);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Generate response with plain token for cookie
        var response = CreateAuthTokensDto(authUser, newRefreshToken, refreshTokenExpiresAt);
        return Result.Success(response);
    }

    private AuthTokensDto CreateAuthTokensDto(AuthUser authUser, string refreshToken, DateTime refreshTokenExpiresAt)
    {
        var roles = authUser.UserRoles
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var permissionsFromRoles = authUser.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name);

        var directPermissions = authUser.UserPermissions
            .Select(up => up.Permission.Name);

        var allPermissions = permissionsFromRoles
            .Concat(directPermissions)
            .Distinct()
            .ToList();

        return new AuthTokensDto
        {
            AuthUserId = authUser.Id.Value,
            Email = authUser.Email,
            Roles = roles,
            Permissions = allPermissions,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
