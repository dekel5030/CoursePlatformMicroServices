using Application.Abstractions.Messaging;
using Application.Users.Commands.UpdateUser;
using Domain.Users.Primitives;
using Kernel;
using Microsoft.AspNetCore.Mvc;
using User.Api.Endpoints;
using User.Api.Extensions;
using User.Api.Infrastructure;

namespace User.Api.Endpoints.Users.Commands;

public class UpdateUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{id:guid}", async (
            Guid id,
            ICommandHandler<UpdateUserCommand, UpdatedUserResponseDto> handler,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken = default) =>
        {
            var command = new UpdateUserCommand(
                id,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.DateOfBirth);

            Result<UpdatedUserResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}

public record UpdateUserRequest(
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    DateTime? DateOfBirth);
