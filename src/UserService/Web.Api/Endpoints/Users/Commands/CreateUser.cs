using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Extensions;
using Users.Api.Infrastructure;
using Users.Application.Users.Commands.CreateUser;

namespace Users.Api.Endpoints.Users.Commands;

internal sealed class CreateUser : IEndpoint
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