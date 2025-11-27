using Application.Abstractions.Messaging;
using Application.Admin.Commands.CreatePermission;
using Application.Admin.Commands.DeletePermission;
using Application.Admin.Dtos;
using Application.Admin.Queries.GetAllPermissions;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Admin;

public class Permissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/permissions", async (
            IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllPermissionsQuery();
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: permissions => Results.Ok(permissions),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("GetAllPermissions")
        .WithSummary("Get all permissions")
        .Produces<IEnumerable<PermissionDto>>(StatusCodes.Status200OK);

        app.MapPost("admin/permissions", async (
            CreatePermissionRequest request,
            ICommandHandler<CreatePermissionCommand, PermissionDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreatePermissionCommand(request);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: permission => Results.Created($"/admin/permissions/{permission.Id}", permission),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("CreatePermission")
        .WithSummary("Create a new permission")
        .Produces<PermissionDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapDelete("admin/permissions/{id:int}", async (
            int id,
            ICommandHandler<DeletePermissionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePermissionCommand(id);
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .WithName("DeletePermission")
        .WithSummary("Delete a permission")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
