using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Queries;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class GetMe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMeQuery();
            Result<UserDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<UserDto>(
            Tags.Users,
            "GetMe",
            "Get current user",
            "Retrieves the currently authenticated user's information");
    }
}
