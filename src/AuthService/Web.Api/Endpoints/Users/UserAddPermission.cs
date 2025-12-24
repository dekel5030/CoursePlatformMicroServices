using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Commands.UserAddPermission;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class UserAddPermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/{userId:guid}/permissions", async (
            Guid userId,
            UserAddPermissionRequestDto request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UserAddPermissionCommand(
                userId,
                request.Effect,
                request.Action,
                request.Resource,
                request.ResourceId
            );

            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(
                onSuccess: () => Results.Ok(),
                onFailure: error => CustomResults.Problem(error));
        })
        .WithMetadata<object>(
            Tags.Users,
            "AddUserPermission",
            "Add permission to user",
            "Assigns a specific permission to an existing user");
    }
}