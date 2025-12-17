using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetUserAuthData;
using Application.Mediator;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Users;

public class GetUserAuthData : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("internal/users/{userId:guid}/auth-data", async (
            Guid userId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserAuthDataQuery(userId);
            var result = await mediator.Send(query, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Internal)
        .WithName("GetUserAuthDataInternal")
        .WithSummary("Internal: Get user permissions and roles")
        .WithDescription("Used by the Gateway to fetch user claims before forwarding requests")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
