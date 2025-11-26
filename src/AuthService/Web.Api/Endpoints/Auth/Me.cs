using System.Security.Claims;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetCurrentUser;
using Auth.Api.Endpoints;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Auth;

public class Me : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/me", async (
            HttpContext httpContext,
            IQueryHandler<GetCurrentUserQuery, AuthResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            // Get email from JWT claims
            var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? httpContext.User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Results.Unauthorized();
            }

            var query = new GetCurrentUserQuery(email);
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: CustomResults.Problem);
        })
        .RequireAuthorization() // This endpoint requires authentication
        .WithTags(Tags.Auth)
        .WithName("GetCurrentUser")
        .WithSummary("Get current authenticated user")
        .WithDescription("Returns the current authenticated user's information from JWT token")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
