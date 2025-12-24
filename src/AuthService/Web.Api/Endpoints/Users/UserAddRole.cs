using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel.Messaging.Abstractions;
using Auth.Application.AuthUsers.Commands.UserAddRole;
using Auth.Application.AuthUsers.Dtos;

namespace Auth.Api.Endpoints.Users;

public class UserAddRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user/{userId:guid}/roles", async (
            Guid userId,
            UserAddRoleRequestDto request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UserAddRoleCommand(userId, request.RoleName);

            var result = await mediator.Send(command, cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Auth)
        .WithName("AddUserRole")
        .WithSummary("Add a role to an existing user")
        .WithDescription("Adds a role to an existing user account")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

