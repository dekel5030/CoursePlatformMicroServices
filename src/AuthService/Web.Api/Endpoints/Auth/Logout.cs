using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.Logout;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Auth;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (
            ICommandHandler<LogoutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LogoutCommand();
            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Logout")
        .WithSummary("Logout user")
        .WithDescription("Clears the authentication cookie and signs the user out")
        .Produces(StatusCodes.Status204NoContent);
    }
}