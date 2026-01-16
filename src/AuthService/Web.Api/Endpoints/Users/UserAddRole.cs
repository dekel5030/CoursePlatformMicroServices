using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Commands.UserAddRole;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

internal sealed class UserAddRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/{userId:guid}/roles", async (
            Guid userId,
            UserAddRoleRequestDto request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UserAddRoleCommand(userId, request.RoleName);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithMetadata<object>(
            Tags.Users,
            "AddUserRole",
            "Add a role to a user",
            "Assigns a specific security role to the provided user identity");
    }
}

