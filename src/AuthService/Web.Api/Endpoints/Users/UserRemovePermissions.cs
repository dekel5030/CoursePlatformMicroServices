using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.UserRemovePermissions;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Users;

public class UserRemovePermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("user/{userId:guid}/permissions/batch", async (
            Guid userId,
            UserRemovePermissionsRequestDto request,
            ICommandHandler<UserRemovePermissionsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UserRemovePermissionsCommand(
                userId,
                request.Permissions.Select(p => new Application.AuthUsers.Commands.UserRemovePermissions.PermissionDto(
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
        .WithTags(Tags.Users)
        .WithName("UserRemovePermissions")
        .WithSummary("Remove multiple permissions from user")
        .WithDescription("Removes multiple permissions from an existing user in a single batch operation")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public record UserRemovePermissionsRequestDto(List<UserRemovePermissionItemDto> Permissions);

public record UserRemovePermissionItemDto(
    string Effect,
    string Action,
    string Resource,
    string ResourceId);
