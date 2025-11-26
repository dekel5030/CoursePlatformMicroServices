using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.Logout;
using Auth.Api.Endpoints;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Auth;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (
            HttpContext httpContext,
            ICommandHandler<LogoutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            string? refreshToken = CookieHelper.GetRefreshTokenFromCookie(httpContext);
            CookieHelper.ClearRefreshCookies(httpContext);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Results.Ok(new { message = "Logged out successfully" });
            }

            var command = new LogoutCommand(refreshToken);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () =>
                {
                    return Results.Ok(new { message = "Logged out successfully" });
                },
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Logout")
        .WithSummary("Logout user")
        .WithDescription("Invalidates the refresh token and clears authentication cookies")
        .Produces(StatusCodes.Status200OK);
    }
}
