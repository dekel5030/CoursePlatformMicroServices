using System.Security.Claims;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.Logout;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (
            HttpContext httpContext,
            ICommandHandler<LogoutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            // Get email from JWT claims
            var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? httpContext.User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                // Clear cookies even if user is not authenticated
                CookieHelper.ClearAuthCookies(httpContext);
                return Results.Ok(new { message = "Logged out successfully" });
            }

            var command = new LogoutCommand(email);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () =>
                {
                    // Clear auth cookies
                    CookieHelper.ClearAuthCookies(httpContext);
                    return Results.Ok(new { message = "Logged out successfully" });
                },
                onFailure: error =>
                {
                    // Even on failure, clear cookies for security
                    CookieHelper.ClearAuthCookies(httpContext);
                    return Results.Ok(new { message = "Logged out successfully" });
                });
        })
        .WithTags(Tags.Auth)
        .WithName("Logout")
        .WithSummary("Logout user")
        .WithDescription("Invalidates the refresh token and clears authentication cookies")
        .Produces(StatusCodes.Status200OK);
    }
}
