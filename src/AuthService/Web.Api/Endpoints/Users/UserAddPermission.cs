using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.UserAddPermission;
using Application.Mediator;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Users;

public class UserAddPermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user/{userId:guid}/permissions", async (
            Guid userId,
            UserAddPermissionRequestDto request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UserAddPermissionCommand(
                userId,
                request.Effect,
                request.Action,
                request.Resource,
                request.ResourceId
            );

            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(
                onSuccess: () => Results.Ok(),
                onFailure: error => CustomResults.Problem(error));
        })
        .WithTags(Tags.Roles)
        .WithName("UserAddPermission")
        .WithSummary("Add permission to user")
        .WithDescription("Adds a permission to an existing user")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}