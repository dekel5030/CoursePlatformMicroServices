using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthTokensResult>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(
        IWriteDbContext dbContext,
        IReadDbContext readDbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _readDbContext = readDbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokensResult>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var dto = request.Dto;

        var authUser = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (authUser == null)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.InvalidCredentials);
        }

        if (authUser.IsLocked())
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.AccountLocked);
        }

        if (!authUser.IsActive)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.AccountInactive);
        }

        if (!_passwordHasher.VerifyPassword(dto.Password, authUser.PasswordHash))
        {
            authUser.RecordFailedLogin();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Failure<AuthTokensResult>(AuthUserErrors.InvalidCredentials);
        }

        // Successful login
        authUser.Login();
        
        // Generate token and response
        var response = CreateAuthTokensResponse(authUser);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(response);
    }

    private AuthTokensResult CreateAuthTokensResponse(AuthUser authUser)
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

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        authUser.SetRefreshToken(refreshTokenHash, refreshTokenExpiresAt);

        var accessToken = _tokenService.GenerateAccessToken(
            new TokenRequestDto { 
                Email = authUser.Email, 
                Permissions = allPermissions, 
                Roles = roles});

        return new AuthTokensResult
        {
            AuthUserId = authUser.Id.Value,
            Email = authUser.Email,
            Roles = roles,
            Permissions = allPermissions,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}