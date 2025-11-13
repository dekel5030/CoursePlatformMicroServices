using Application.Abstractions.Messaging;
using Application.Lessons.Commands.CreateLesson;
using Domain.Lessons.Primitives;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Lessons;

public class CreateLesson : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/lessons", async (
            CreateLessonDto dto,
            ICommandHandler<CreateLessonCommand, LessonId> handler) =>
        {
            var result = await handler.Handle(new CreateLessonCommand(dto));

            return result.Match(
                lessonId => Results.Created(
                    $"/api/lessons/{lessonId.Value}",
                    new { Id = lessonId.Value }
                ),
                CustomResults.Problem);
        })
        .WithName("CreateLesson")
        .WithTags("Lessons");
    }
}
