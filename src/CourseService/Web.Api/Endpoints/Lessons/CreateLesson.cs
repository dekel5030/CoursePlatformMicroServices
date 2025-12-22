using Application.Lessons.Commands.CreateLesson;
using Course.Api.Extensions;
using Course.Api.Infrastructure;
using Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Course.Api.Endpoints.Lessons;

public class CreateLesson : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("lessons", async (
            CreateLessonDto dto,
            ICommandHandler<CreateLessonCommand, LessonId> handler) =>
        {
            var result = await handler.Handle(new CreateLessonCommand(dto));

            return result.Match(
                lessonId => Results.CreatedAtRoute(
                    "GetLessonById",
                    new { id = lessonId.Value },
                    new { Id = lessonId.Value }
                ),
                CustomResults.Problem);
        })
        .WithName("CreateLesson")
        .WithTags("Lessons");
    }
}
