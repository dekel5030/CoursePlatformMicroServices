using Application.AuthUsers.Commands.ExchangeToken;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Infrastructure.Auth.Jwt;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Auth;

public class ExchangeToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/exchange-token", async (
            IMediator mediator) =>
        {
            var result = await mediator.Send(new ExchangeTokenCommand());
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(AuthSchemes.Keycloak);
    }
}