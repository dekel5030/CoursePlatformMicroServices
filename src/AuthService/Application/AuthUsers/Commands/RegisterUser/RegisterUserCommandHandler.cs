using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Roles;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthTokensResult>
{
    private readonly IWriteDbContext _dbContext;
    private readonly IReadDbContext _readDbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(
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
        RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var dto = request.Dto;

        // Check if user already exists
        var existingUser = await _readDbContext.AuthUsers
            .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (existingUser != null)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.DuplicateEmail);
        }

        // Hash password
        string passwordHash = _passwordHasher.Hash(dto.Password);

        // Get default role
        var defaultRole = await _readDbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);

        if (defaultRole == null)
        {
            return Result.Failure<AuthTokensResult>(
                Error.NotFound("Role.NotFound", "Default 'User' role not found"));
        }

        // Create auth user (UserId will be set asynchronously when UserService responds)
        var authUser = AuthUser.Create(
            dto.Email,
            passwordHash,
            defaultRole.Id);

        await _dbContext.AuthUsers.AddAsync(authUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Load user with roles and permissions for token generation
        var fullAuthUser = await _readDbContext.AuthUsers
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == authUser.Id, cancellationToken);

        if (fullAuthUser == null)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.NotFound);
        }

        // Generate token and response
        var response = CreateAuthTokensDto(fullAuthUser);
        await _dbContext.SaveChangesAsync(cancellationToken); // Save refresh token
        return Result.Success(response);
    }

    private AuthTokensResult CreateAuthTokensDto(AuthUser authUser)
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

        // Generate refresh token
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7); // 7 days expiration
        
        // Hash the refresh token before storing in the database
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        authUser.SetRefreshToken(refreshTokenHash, refreshTokenExpiresAt);

        return new AuthTokensResult
        {
            AuthUserId = authUser.Id.Value,
            Email = authUser.Email,
            Roles = roles,
            Permissions = allPermissions,
            AccessToken = null!,
            RefreshToken = refreshToken, // Return the plain token to set in cookie
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
