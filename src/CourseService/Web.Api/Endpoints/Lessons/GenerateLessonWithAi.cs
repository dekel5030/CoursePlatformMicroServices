using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.GenerateLessonWithAi;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GenerateLessonWithAi : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("lessons/{lessonId:Guid}/ai-generate", async (
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var lessonIdObj = new LessonId(lessonId);
            var command = new GenerateLessonWithAiCommand(lessonIdObj);

            Result<GenerateLessonWithAiResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<GenerateLessonWithAiResponse>(
            nameof(GenerateLessonWithAi),
            tag: Tags.Lessons,
            summary: "Generate lesson data with AI");
    }
}
