using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Extensions;
using Users.Api.Infrastructure;
using Users.Application.Users.Commands.UpdateUser;
using Users.Domain.Users.Primitives;

namespace Users.Api.Endpoints.Users.Commands;

internal sealed class UpdateUser : IEndpoint
{
    internal sealed record UpdateUserRequest(
        string? FirstName,
        string? LastName,
        PhoneNumber? PhoneNumber,
        DateTime? DateOfBirth);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/users/{id:guid}", async (
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
        }).RequireAuthorization();
    }
}