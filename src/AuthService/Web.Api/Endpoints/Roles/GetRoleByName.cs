using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.Roles.Queries.GetRoleByName;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Roles;

public class GetRoleByName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/roles/{roleName}", async (
            string roleName,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRoleByNameQuery(roleName);
            Result<RoleDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
