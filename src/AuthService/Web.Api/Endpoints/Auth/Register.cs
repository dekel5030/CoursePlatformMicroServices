using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Dtos;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;

namespace Auth.Api.Endpoints.Auth;

public class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
            RegisterRequestDto request,
            ICommandHandler<RegisterUserCommand, CurrentUserDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(request);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Register")
        .WithSummary("Register a new user")
        .WithDescription("Creates a new user account and set cookies")
        .Produces<CurrentUserDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
