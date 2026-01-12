using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Auth.Application.AuthUsers.Commands.ExchangeToken;
using Auth.Infrastructure.Auth.Jwt;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Auth.Api.Endpoints.Auth;

internal sealed class ExchangeToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/exchange-token", async (
            IMediator mediator) =>
        {
            Result<TokenResponse> result = await mediator.Send(new ExchangeTokenCommand());
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(AuthSchemes.Keycloak)
        .WithTags(Tags.Auth)
        .WithName("ExchangeToken")
        .WithSummary("Exchange external token for internal token")
        .WithDescription("Exchanges an external authentication token for an internal system token")
        .Produces<TokenResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
