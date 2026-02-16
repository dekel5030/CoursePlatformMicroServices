using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Commands.UpdateLessonProgress;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class UpdateEnrollmentProgress : IEndpoint
{
    internal sealed record UpdateEnrollmentProgressRequest(
        Guid LessonId,
        int Seconds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("enrollments/{id:Guid}/progress", async (
            Guid id,
            UpdateEnrollmentProgressRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLessonProgressCommand(
                new EnrollmentId(id),
                new LessonId(request.LessonId),
                request.Seconds);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithName("UpdateEnrollmentProgress")
        .WithMetadata<EmptyResult>(nameof(UpdateEnrollmentProgress), Tags.Enrollments, "Updates the video offset for an enrollment's lesson progress.");
    }
}
