using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.Roles.Commands.RoleAddPermission;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Roles;

internal sealed class AddRolePermission : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles/{roleName}/permissions", async (
            string roleName,
            RoleAddPermissionRequestDto request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new RoleAddPermissionCommand(
                roleName,
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
        .WithMetadata<object>(
            Tags.Roles,
            "AddRolePermission",
            "Add permission to role",
            "Assigns a specific permission to an existing security role");
    }
}