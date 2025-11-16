using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Dtos;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            LoginRequestDto request,
            ICommandHandler<LoginUserCommand, AuthResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: response => Results.Ok(response),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Login")
        .WithSummary("Login user")
        .WithDescription("Authenticates a user and returns authentication token")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
