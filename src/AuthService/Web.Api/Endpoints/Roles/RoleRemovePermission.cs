using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.Roles.Commands.RoleRemovePermission;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Roles;

public class RoleRemovePermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("roles/{roleName}/permissions/{permissionKey}", async (
            string roleName,
            string permissionKey,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new RoleRemovePermissionCommand(roleName, permissionKey);

            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => CustomResults.Problem(error));
        })
        .WithTags(Tags.Roles)
        .WithName("RemoveRolePermission")
        .WithSummary("Remove permission from role")
        .WithDescription("Removes a permission from an existing role")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}