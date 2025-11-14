using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthResponseDto>
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

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var dto = request.Dto;

        // Check if user already exists
        var existingUser = await _readDbContext.AuthUsers
            .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (existingUser != null)
        {
            return Result.Failure<AuthResponseDto>(AuthUserErrors.DuplicateEmail);
        }

        // Hash password
        string passwordHash = _passwordHasher.Hash(dto.Password);

        // Get default role
        var defaultRole = await _readDbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);

        if (defaultRole == null)
        {
            return Result.Failure<AuthResponseDto>(
                Error.NotFound("Role.NotFound", "Default 'User' role not found"));
        }

        // For now, we'll use a placeholder userId - in event-driven architecture,
        // this would be handled via integration events from UserService
        // TODO: Implement proper event-driven user creation
        int temporaryUserId = 0; // This should come from UserService via event

        // Create auth user
        var authUser = AuthUser.Create(
            temporaryUserId,
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
            .FirstOrDefaultAsync(u => u.Id.Value == authUser.Id.Value, cancellationToken);

        if (fullAuthUser == null)
        {
            return Result.Failure<AuthResponseDto>(AuthUserErrors.NotFound);
        }

        // Generate token and response
        var response = CreateAuthResponseDto(fullAuthUser);
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
