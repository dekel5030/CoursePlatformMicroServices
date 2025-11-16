using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

public class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
            RegisterRequestDto request,
            ICommandHandler<RegisterUserCommand, AuthResponseDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(request);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: response => Results.Ok(response),
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Register")
        .WithSummary("Register a new user")
        .WithDescription("Creates a new user account and returns authentication token")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
