using Application.AuthUsers.Commands.ProvisionUser;
using Application.AuthUsers.Dtos;
using Application.Mediator;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Users;

public class ProvisionUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("internal/provision/{userIdentityId}", async (
            string userIdentityId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new ProvisionUserCommand(userIdentityId);
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Internal)
        .WithName("ProvisionUserInternal")
        .WithSummary("Internal: Provision a new user")
        .WithDescription("Used by the Gateway to provision a new user")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
