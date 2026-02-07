using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Extensions;
using Users.Api.Infrastructure;
using Users.Application.LecturerProfiles.Commands.CreateLecturerProfile;

namespace Users.Api.Endpoints.LecturerProfiles.Commands;

internal sealed class CreateLecturerProfile : IEndpoint
{
    internal sealed record CreateLecturerProfileRequest(
        string? ProfessionalBio,
        string? Expertise,
        int YearsOfExperience);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{userId:guid}/lecturer-profile", async (
            Guid userId,
            ICommandHandler<CreateLecturerProfileCommand, LecturerProfileResponseDto> handler,
            [FromBody] CreateLecturerProfileRequest request,
            CancellationToken cancellationToken = default) =>
        {
            var command = new CreateLecturerProfileCommand(
                userId,
                request.ProfessionalBio,
                request.Expertise,
                request.YearsOfExperience);

            Result<LecturerProfileResponseDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization();
    }
}
