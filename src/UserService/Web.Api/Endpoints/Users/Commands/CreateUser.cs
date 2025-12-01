using Application.Abstractions.Messaging;
using Application.Users.Commands.CreateUser;
using Kernel;
using Microsoft.AspNetCore.Mvc;
using User.Api.Endpoints;
using User.Api.Extensions;
using User.Api.Infrastructure;

namespace User.Api.Endpoints.Users.Commands;

public class CreateUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/", async (
            ICommandHandler<CreateUserCommand, CreatedUserRespondDto> handler,
            [FromBody] CreateUserCommand command,
            CancellationToken cancellationToken = default) =>
        {
            Result<CreatedUserRespondDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                value => Results.CreatedAtRoute(
                    routeName: "GetUserById",
                    routeValues: new { id = value.Id },
                    value: value),
                CustomResults.Problem);
        });
    }
}