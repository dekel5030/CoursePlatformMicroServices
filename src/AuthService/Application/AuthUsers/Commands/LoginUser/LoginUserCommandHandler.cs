using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Kernel;
using Microsoft.AspNetCore.Identity;

namespace Application.AuthUsers.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthTokensResult>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AuthUser> _userManager;

    public LoginUserCommandHandler(
        IWriteDbContext dbContext,
        ITokenService tokenService,
        UserManager<AuthUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<Result<AuthTokensResult>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var requestDto = request.Dto;

        var user = await _userManager.FindByEmailAsync(requestDto.Email);

        if (user == null)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.InvalidCredentials);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.AccountLocked);
        }

        if (!user.EmailConfirmed)
        {
            return Result.Failure<AuthTokensResult>(AuthUserErrors.AccountInactive);
        }

        bool correctPassword = await _userManager.CheckPasswordAsync(user, requestDto.Password);

        if (!correctPassword)
        {
            await _userManager.AccessFailedAsync(user);
            return Result.Failure<AuthTokensResult>(AuthUserErrors.InvalidCredentials);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var response = await CreateAuthTokensResponse(user);
        
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
                Permissions = userClaims, 
                Roles = roles});

        return new AuthTokensResult
        {
            AuthUserId = authUser.Id,
            Email = authUser.Email!,
            Roles = roles,
            Permissions = userClaims,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}