using Application.Abstractions.Messaging;
using Application.Roles.Commands.RoleAddPermission;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Roles;

public class RoleAddPermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles/{roleId:guid}/permissions", async (
            Guid roleId,
            RoleAddPermissionRequestDto request,
            ICommandHandler< RoleAddPermissionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RoleAddPermissionCommand(
                roleId,
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
        .WithName("AddRolePermission")
        .WithSummary("Add permission to role")
        .WithDescription("Adds a permission to an existing role")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}