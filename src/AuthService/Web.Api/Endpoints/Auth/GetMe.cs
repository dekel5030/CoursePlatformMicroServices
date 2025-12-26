using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Queries;
using Auth.Infrastructure.Auth.Jwt;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Api.Endpoints.Auth;

public class GetMe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMeQuery();
            Result<UserDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(AuthSchemes.Keycloak)
        .WithMetadata<UserDto>(
            Tags.Auth,
            "GetMe",
            "Get current user profile",
            "Retrieves the authenticated user's profile with enriched data from the AuthService database");
    }
}
