using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Commands.RefreshAccessToken;
using Application.AuthUsers.Dtos;
using Auth.Api.Endpoints;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Auth;

public class RefreshAccessToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh-token", async (
            ICommandHandler<RefreshAccessTokenCommand, string> handler,
            ITokenService tokenService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var refreshToken = CookieHelper.GetRefreshTokenFromCookie(httpContext);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.Problem(
                    title: "RefreshToken.Missing",
                    detail: "Refresh token not found in cookies",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var command = new RefreshAccessTokenCommand(refreshToken);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: accessToken =>
                {
                    return Results.Ok(accessToken);
                },
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("RefreshAccessToken")
        .WithSummary("Refresh access token")
        .WithDescription("Validates refresh token from cookie and issues new access and refresh tokens")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
