using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.UserAddPermissions;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Users;

public class UserAddPermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user/{userId:guid}/permissions/batch", async (
            Guid userId,
            UserAddPermissionsRequestDto request,
            ICommandHandler<UserAddPermissionsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UserAddPermissionsCommand(
                userId,
                request.Permissions.Select(p => new Application.AuthUsers.Commands.UserAddPermissions.PermissionDto(
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
        .WithName("UserAddPermissions")
        .WithSummary("Add multiple permissions to user")
        .WithDescription("Adds multiple permissions to an existing user in a single batch operation")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public record UserAddPermissionsRequestDto(List<UserAddPermissionItemDto> Permissions);

public record UserAddPermissionItemDto(
    string Effect,
    string Action,
    string Resource,
    string? ResourceId = null);
