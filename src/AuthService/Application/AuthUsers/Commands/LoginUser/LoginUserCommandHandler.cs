using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResponseDto>
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

    public async Task<Result<AuthResponseDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var dto = request.Dto;

        // Get user with all access data
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
            return Result.Failure<AuthResponseDto>(AuthUserErrors.InvalidCredentials);
        }

        // Check if account is locked
        if (authUser.IsLocked())
        {
            return Result.Failure<AuthResponseDto>(AuthUserErrors.AccountLocked);
        }

        // Check if account is active
        if (!authUser.IsActive)
        {
            return Result.Failure<AuthResponseDto>(AuthUserErrors.AccountInactive);
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(dto.Password, authUser.PasswordHash))
        {
            // Record failed login attempt
            authUser.RecordFailedLogin();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Failure<AuthResponseDto>(AuthUserErrors.InvalidCredentials);
        }

        // Successful login
        authUser.Login();
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Generate token and response
        var response = CreateAuthResponseDto(authUser);
        return Result.Success(response);
    }

    private AuthResponseDto CreateAuthResponseDto(AuthUser authUser)
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

        var tokenRequest = new TokenRequestDto
        {
            Email = authUser.Email,
            Roles = roles,
            Permissions = allPermissions
        };

        var token = _tokenService.GenerateToken(tokenRequest);

        return new AuthResponseDto
        {
            AuthUserId = authUser.Id.Value,
            Email = authUser.Email,
            Token = token,
            Roles = roles,
            Permissions = allPermissions
        };
    }
}
