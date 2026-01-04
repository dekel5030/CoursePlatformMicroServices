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
        app.MapDelete("lessons/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLessonCommand(id);
            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata(nameof(DeleteLesson), Tags.Lessons, "Deletes a lesson by its ID.", 204);
    }
}
