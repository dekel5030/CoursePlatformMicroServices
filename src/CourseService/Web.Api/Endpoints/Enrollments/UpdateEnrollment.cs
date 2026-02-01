using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Commands.UpdateEnrollment;
using Courses.Domain.Enrollments.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class UpdateEnrollment : IEndpoint
{
    internal sealed record UpdateEnrollmentRequest(
        DateTimeOffset? ExpiresAt,
        bool? Revoke);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enrollments/{id:Guid}", async (
            Guid id,
            UpdateEnrollmentRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateEnrollmentCommand(
                new EnrollmentId(id),
                request.ExpiresAt,
                request.Revoke);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(nameof(UpdateEnrollment), Tags.Enrollments, "Updates an enrollment.");
    }
}
