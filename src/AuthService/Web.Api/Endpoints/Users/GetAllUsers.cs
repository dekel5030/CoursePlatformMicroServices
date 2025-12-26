using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Queries.Dtos;
using Auth.Application.AuthUsers.Queries.GetAllUsers;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class GetAllUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllUsersQuery();
            Result<IReadOnlyList<UserDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<IReadOnlyList<UserDto>>(
            Tags.Users,
            "GetAllUsers",
            "Get all users",
            "Retrieves all users in the system");
    }
}
