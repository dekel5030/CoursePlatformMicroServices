using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Dtos;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Auth;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            LoginRequestDto request,
            ICommandHandler<LoginUserCommand, LoginResultDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request);

            Result<LoginResultDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Login")
        .WithSummary("Login user")
        .WithDescription("Authenticates a user and returns user information via JSON + HttpOnly Cookie")
        .Produces<LoginResultDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}