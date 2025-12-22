using Application.AuthUsers.Commands.UserRemovePermission;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class UserRemovePermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("user/{userId:guid}/permissions/", async (
            Guid userId,
            [AsParameters] UserRemovePermissionRequestDto request,
            ICommandHandler<UserRemovePermissionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UserRemovePermissionCommand(
                userId,
                request.Effect,
                request.Action,
                request.Resource,
                request.ResourceId
            );

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                onSuccess: () => Results.Ok(),
                onFailure: error => CustomResults.Problem(error));
        })
        .WithTags(Tags.Roles)
        .WithName("UserRemovePermission")
        .WithSummary("Remove permission from user")
        .WithDescription("Removes a permission from an existing user")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}