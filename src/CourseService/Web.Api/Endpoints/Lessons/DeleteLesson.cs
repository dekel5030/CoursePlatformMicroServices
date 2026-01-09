using CoursePlatform.ServiceDefaults.CustomResults;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.DeleteLesson;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

public class DeleteLesson : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("courses/{courseId:CourseId}/lessons/{lessonId:LessonId}", async (
            CourseId courseId,
            LessonId lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLessonCommand(courseId, lessonId);
            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata(nameof(DeleteLesson), Tags.Lessons, "Deletes a specific lesson from a given course.", 204);
    }
}
