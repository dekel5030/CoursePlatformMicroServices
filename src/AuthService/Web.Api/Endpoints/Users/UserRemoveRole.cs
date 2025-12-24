using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel.Messaging.Abstractions;
using Auth.Application.AuthUsers.Dtos;
using Auth.Application.AuthUsers.Commands.UserRemoveRole;

namespace Auth.Api.Endpoints.Users;

public class UserRemoveRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("users/{userId:guid}/roles/{roleName}", async (
            Guid userId,
            string roleName,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UserRemoveRoleCommand(userId, roleName);

            var result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<object>(
            Tags.Users,
            "RemoveUserRole",
            "Remove a role from a user",
            "Removes a specific security role from an existing user account");
    }
}

