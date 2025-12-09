using Application.AuthUsers.Dtos;
using Application.Abstractions.Messaging;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Application.AuthUsers.Commands.AddRole;

namespace Auth.Api.Endpoints.Auth;

public class UserAddRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/user/{userId:guid}/roles", async (
            Guid userId,
            UserAddRoleRequestDto request,
            ICommandHandler<UserAddRoleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UserAddRoleCommand(userId, request.RoleName);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("AddUserRole")
        .WithSummary("Add a role to an existing user")
        .WithDescription("Adds a role to an existing user account")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

