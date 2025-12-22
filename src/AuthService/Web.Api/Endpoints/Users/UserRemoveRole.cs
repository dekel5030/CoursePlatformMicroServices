using Application.AuthUsers.Dtos;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Application.AuthUsers.Commands.UserRemoveRole;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class UserRemoveRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("user/{userId:guid}/roles/{roleName}", async (
            Guid userId,
            string roleName,
            ICommandHandler<UserRemoveRoleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UserRemoveRoleCommand(userId, roleName);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("RemoveUserRole")
        .WithSummary("Remove a role from an existing user")
        .WithDescription("Removes a role from an existing user account")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

