using Application.Abstractions.Messaging;
using Application.Admin.Commands.AssignPermissionToRole;
using Application.Admin.Commands.CreateRole;
using Application.Admin.Commands.DeleteRole;
using Application.Admin.Commands.RemovePermissionFromRole;
using Application.Admin.Dtos;
using Application.Admin.Queries.GetAllRoles;
using Application.Admin.Queries.GetRolePermissions;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Admin;

public class Roles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/roles", async (
            IQueryHandler<GetAllRolesQuery, IEnumerable<RoleDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllRolesQuery();
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: roles => Results.Ok(roles),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("GetAllRoles")
        .WithSummary("Get all roles")
        .Produces<IEnumerable<RoleDto>>(StatusCodes.Status200OK);

        app.MapPost("admin/roles", async (
            CreateRoleRequest request,
            ICommandHandler<CreateRoleCommand, RoleDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateRoleCommand(request);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: role => Results.Created($"/admin/roles/{role.Id}", role),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("CreateRole")
        .WithSummary("Create a new role")
        .Produces<RoleDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapDelete("admin/roles/{id:int}", async (
            int id,
            ICommandHandler<DeleteRoleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteRoleCommand(id);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("DeleteRole")
        .WithSummary("Delete a role")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet("admin/roles/{id:int}/permissions", async (
            int id,
            IQueryHandler<GetRolePermissionsQuery, IEnumerable<PermissionDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRolePermissionsQuery(id);
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: permissions => Results.Ok(permissions),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("GetRolePermissions")
        .WithSummary("Get permissions for a role")
        .Produces<IEnumerable<PermissionDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost("admin/roles/{roleId:int}/permissions/{permissionId:int}", async (
            int roleId,
            int permissionId,
            ICommandHandler<AssignPermissionToRoleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AssignPermissionToRoleCommand(new AssignPermissionToRoleRequest(roleId, permissionId));
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("AssignPermissionToRole")
        .WithSummary("Assign a permission to a role")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete("admin/roles/{roleId:int}/permissions/{permissionId:int}", async (
            int roleId,
            int permissionId,
            ICommandHandler<RemovePermissionFromRoleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemovePermissionFromRoleCommand(roleId, permissionId);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("RemovePermissionFromRole")
        .WithSummary("Remove a permission from a role")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
