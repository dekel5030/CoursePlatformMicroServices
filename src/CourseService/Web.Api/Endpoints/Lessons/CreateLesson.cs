using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Lessons.Commands.CreateLesson;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

public class CreateLesson : IEndpoint
{
    public record CreateLessonRequest(string? Title, string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{courseid:guid}/lessons", async (
            Guid courseid,
            CreateLessonRequest request,
            IMediator mediator) =>
        {
            var command = new CreateLessonCommand(
                courseid,
                request.Title,
                request.Description);

            var result = await mediator.Send(command);

            return result.Match(
                lessonDto => Results.CreatedAtRoute(
                    "GetLessonById",
                    new { id = lessonDto.Id },
                    lessonDto
                ),
                CustomResults.Problem);
        })
        .WithName("CreateLesson")
        .WithTags(Tags.Lessons);
    }
}
