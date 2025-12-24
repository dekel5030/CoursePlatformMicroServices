using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Queries;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserByIdQuery(id);
            Result<UserDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .WithName("UserGetById")
        .WithSummary("Get user by ID")
        .WithDescription("Retrieves a user by their unique identifier");
    }
}
