using System.Security.Claims;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetCurrentUser;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Auth;

public class Me : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/me", async (
            ClaimsPrincipal user,
            IQueryHandler<GetCurrentUserQuery, CurrentUserDto> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out Guid userId);

            if (userId == Guid.Empty)
            {
                return Results.Unauthorized();
            }
            var query = new GetCurrentUserQuery(userId);
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Auth)
        .WithName("GetCurrentUser")
        .WithSummary("Get current authenticated user")
        .WithDescription("Returns the current authenticated user's information based on the active session cookie")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
