using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.Roles.Queries.GetAllRoles;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Roles;

public class GetAllRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/roles", async (
            IMediator mediator) =>
        {
            var query = new GetAllRolesQuery();

            var result = await mediator.Send(query);

            return result.Match(
                roles => Results.Ok(roles),
                error => CustomResults.Problem(error)
            );
        })
        .WithMetadata<IReadOnlyCollection<RoleDto>>(
            Tags.Roles,
            "GetAllRoles",
            "Get all roles",
            "Retrieves all security roles defined in the system");
    }
}
