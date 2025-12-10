using Application.Abstractions.Messaging;
using Application.Roles.Commands.AddPermissionsToRole;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Roles;

public class AddPermissionsToRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles/{roleId:guid}/permissions/batch", async (
            Guid roleId,
            AddPermissionsToRoleRequestDto request,
            ICommandHandler<RoleAddPermissionsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RoleAddPermissionsCommand(
                roleId,
                request.Permissions.Select(p => new Application.Roles.Commands.AddPermissionsToRole.PermissionDto(
                    p.Effect,
                    p.Action,
                    p.Resource,
                    p.ResourceId
                )).ToList()
            );

            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: error => CustomResults.Problem(error));
        })
        .WithTags(Tags.Roles)
        .WithName("AddPermissionsToRole")
        .WithSummary("Add multiple permissions to role")
        .WithDescription("Adds multiple permissions to an existing role in a single batch operation")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public record AddPermissionsToRoleRequestDto(List<RolePermissionItemDto> Permissions);

public record RolePermissionItemDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
