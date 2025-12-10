using Application.Abstractions.Messaging;
using Application.Roles.Commands.RoleRemovePermissions;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Roles;

public class RoleRemovePermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("roles/{roleId:guid}/permissions/batch", async (
            Guid roleId,
            RoleRemovePermissionsRequestDto request,
            ICommandHandler<RoleRemovePermissionsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RoleRemovePermissionsCommand(
                roleId,
                request.Permissions
            );

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => CustomResults.Problem(error));
        })
        .WithTags(Tags.Roles)
        .WithName("RemovePermissionsFromRole")
        .WithSummary("Remove multiple permissions from role")
        .WithDescription("Removes multiple permissions from an existing role in a single batch operation")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
