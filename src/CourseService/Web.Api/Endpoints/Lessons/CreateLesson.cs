using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Lessons.Commands.CreateLesson;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

public class CreateLesson : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("lessons", async (
            CreateLessonDto dto,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateLessonCommand(dto));

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
