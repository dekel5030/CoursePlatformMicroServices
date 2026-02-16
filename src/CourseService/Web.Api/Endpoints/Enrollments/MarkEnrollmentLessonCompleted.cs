using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Commands.MarkLessonAsCompleted;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class MarkEnrollmentLessonCompleted : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("enrollments/{id:Guid}/lessons/{lessonId:Guid}/completed", async (
            Guid id,
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new MarkLessonAsCompletedCommand(
                new EnrollmentId(id),
                new LessonId(lessonId));

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithName("MarkEnrollmentLessonCompleted")
        .WithMetadata<EmptyResult>(nameof(MarkEnrollmentLessonCompleted), Tags.Enrollments, "Marks a lesson as completed for an enrollment.");
    }
}
