using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.Roles.Commands.CreateRole;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Roles;

internal sealed class CreateRole : IEndpoint
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
        .WithMetadata<CreateRoleResponseDto>(
            Tags.Roles,
            "CreateRole",
            "Create a new role",
            "Creates a new security role in the system");
    }
}