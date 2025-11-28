using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthTokensResult>
{
    private readonly IWriteDbContext _dbContext;
    private readonly UserManager<AuthUser> _userManager;
    private readonly RoleManager<Domain.Roles.Role> _roleManager;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(
        IWriteDbContext dbContext,
        UserManager<AuthUser> userManager,
        RoleManager<Domain.Roles.Role> roleManager,
        ITokenService tokenService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokensResult>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var dto = request.Dto;

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.DuplicateEmail);
        }

        // Create auth user using Identity
        var authUser = AuthUser.Create(dto.Email, dto.Email);

        var createResult = await _userManager.CreateAsync(authUser, dto.Password);
        if (!createResult.Succeeded)
        {
            var errorMessage = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthTokensResult>(
                Error.Validation("Registration.Failed", errorMessage));
        }

        // Assign default "User" role
        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(authUser, "User");
        }

        // Generate token and response
        var response = await CreateAuthTokensResponse(authUser);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(response);
    }

    private async Task<AuthTokensResult> CreateAuthTokensResponse(AuthUser authUser)
    {
        var userClaims = (await _userManager.GetClaimsAsync(authUser)).Select(claim => claim.ToString());
        var roles = await _userManager.GetRolesAsync(authUser);

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        await _userManager
            .SetAuthenticationTokenAsync(authUser, "AuthService", "refreshToken", refreshTokenHash);

        var accessToken = _tokenService.GenerateAccessToken(
            new TokenRequestDto { 
                Email = authUser.Email!, 
                Permissions = userClaims.AsEnumerable(), 
                Roles = roles});

        return new AuthTokensResult
        {
            AuthUserId = authUser.Id,
            Email = authUser.Email!,
            Roles = roles,
            Permissions = userClaims.ToArray(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
