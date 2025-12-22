using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.Roles.Commands.CreateRole;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Roles;

public class CreateRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles", async (
            CreateRoleCommand command,
            ICommandHandler<CreateRoleCommand, CreateRoleResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            Result<CreateRoleResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Roles)
        .WithName("CreateRole")
        .WithSummary("Create role")
        .WithDescription("Creates a new role in the system")
        .Produces<CreateRoleResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}