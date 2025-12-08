using Application.Abstractions.Messaging;
using Application.Roles.AddRolePermission;
using Application.Roles.CreateRole;
using Application.Roles.RemoveRolePermission;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Domain.Roles.Primitives;
using Kernel;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Endpoints.Roles;

public class RemoveRolePermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("roles/{roleId:guid}/permissions", async (
            Guid roleId,
            [FromBody] RemoveRolePermissionRequestDto request,
            ICommandHandler<RemoveRolePermissionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveRolePermissionCommand(
                roleId,
                request.Effect,
                request.Action,
                request.Resource,
                request.ResourceId
            );

            Result result = await handler.Handle(command, cancellationToken);
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