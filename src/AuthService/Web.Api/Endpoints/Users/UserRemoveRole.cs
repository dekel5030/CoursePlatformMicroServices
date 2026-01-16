using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Commands.UserRemoveRole;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

internal sealed class UserRemoveRole : IEndpoint
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

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users)
        .WithName("RemoveUserRole")
        .WithSummary("Remove a role from a user")
        .WithDescription("Removes a specific security role from an existing user account")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

