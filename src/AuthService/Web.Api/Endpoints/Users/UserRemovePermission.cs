using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Commands.UserRemovePermission;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Users;

public class UserRemovePermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("user/{userId:guid}/permissions/{permissionKey}", async (
            Guid userId,
            string permissionKey,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UserRemovePermissionCommand(
                userId,
                permissionKey
            );

            Result result = await mediator.Send(command, cancellationToken);
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