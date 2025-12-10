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
            IQueryHandler<GetCurrentUserQuery, CurrentUserDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCurrentUserQuery();
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
