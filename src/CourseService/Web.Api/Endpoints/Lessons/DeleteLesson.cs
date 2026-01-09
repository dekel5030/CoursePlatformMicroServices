using CoursePlatform.ServiceDefaults.CustomResults;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.DeleteLesson;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

public class DeleteLesson : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("courses/{courseId:Guid}/lessons/{lessonId:Guid}", async (
            Guid courseId,
            Guid lessonId,
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
