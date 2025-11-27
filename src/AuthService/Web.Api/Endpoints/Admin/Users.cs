using Application.Abstractions.Messaging;
using Application.Admin.Commands.AssignPermissionToUser;
using Application.Admin.Commands.AssignRoleToUser;
using Application.Admin.Commands.RemovePermissionFromUser;
using Application.Admin.Commands.RemoveRoleFromUser;
using Application.Admin.Dtos;
using Application.Admin.Queries.GetAllUsers;
using Application.Admin.Queries.GetUserPermissions;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Admin;

public class Users : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/users", async (
            IQueryHandler<GetAllUsersQuery, IEnumerable<UserDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllUsersQuery();
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: users => Results.Ok(users),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("GetAllUsers")
        .WithSummary("Get all users")
        .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK);

        app.MapGet("admin/users/{userId:guid}/permissions", async (
            Guid userId,
            IQueryHandler<GetUserPermissionsQuery, IEnumerable<PermissionDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserPermissionsQuery(userId);
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: permissions => Results.Ok(permissions),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("GetUserPermissions")
        .WithSummary("Get direct permissions for a user")
        .Produces<IEnumerable<PermissionDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost("admin/users/{userId:guid}/permissions/{permissionId:int}", async (
            Guid userId,
            int permissionId,
            ICommandHandler<AssignPermissionToUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AssignPermissionToUserCommand(new AssignPermissionToUserRequest(userId, permissionId));
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("AssignPermissionToUser")
        .WithSummary("Assign a permission to a user")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete("admin/users/{userId:guid}/permissions/{permissionId:int}", async (
            Guid userId,
            int permissionId,
            ICommandHandler<RemovePermissionFromUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemovePermissionFromUserCommand(userId, permissionId);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("RemovePermissionFromUser")
        .WithSummary("Remove a permission from a user")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost("admin/users/{userId:guid}/roles/{roleId:int}", async (
            Guid userId,
            int roleId,
            ICommandHandler<AssignRoleToUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AssignRoleToUserCommand(new AssignRoleToUserRequest(userId, roleId));
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("AssignRoleToUser")
        .WithSummary("Assign a role to a user")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete("admin/users/{userId:guid}/roles/{roleId:int}", async (
            Guid userId,
            int roleId,
            ICommandHandler<RemoveRoleFromUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveRoleFromUserCommand(userId, roleId);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("RemoveRoleFromUser")
        .WithSummary("Remove a role from a user")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
