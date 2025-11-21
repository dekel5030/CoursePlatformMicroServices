using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Commands.RefreshToken;
using Application.AuthUsers.Dtos;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh-token", async (
            ICommandHandler<RefreshTokenCommand, AuthTokensDto> handler,
            ITokenService tokenService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            // Get refresh token from cookie
            var refreshToken = CookieHelper.GetRefreshTokenFromCookie(httpContext);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.Problem(
                    title: "RefreshToken.Missing",
                    detail: "Refresh token not found in cookies",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var command = new RefreshTokenCommand(refreshToken);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: tokensDto =>
                {
                    // Set new cookies with rotated tokens
                    CookieHelper.SetAuthCookies(
                        httpContext,
                        tokenService,
                        tokensDto.Email,
                        tokensDto.Roles,
                        tokensDto.Permissions,
                        tokensDto.RefreshToken,
                        tokensDto.RefreshTokenExpiresAt);
                    
                    // Return public response without tokens
                    var response = new AuthResponseDto
                    {
                        AuthUserId = tokensDto.AuthUserId,
                        UserId = tokensDto.AuthUserId, // Unified ID
                        Email = tokensDto.Email,
                        Roles = tokensDto.Roles,
                        Permissions = tokensDto.Permissions,
                        Message = "Token refreshed successfully"
                    };
                    
                    return Results.Ok(response);
                },
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("RefreshToken")
        .WithSummary("Refresh access token")
        .WithDescription("Validates refresh token from cookie and issues new access and refresh tokens")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
