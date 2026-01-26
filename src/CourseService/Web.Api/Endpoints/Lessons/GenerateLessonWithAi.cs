using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.GenerateLessonWithAi;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GenerateLessonWithAi : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("modules/{moduleId:Guid}/lessons/{lessonId:Guid}/ai-generate", async (
            Guid moduleId,
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var moduleIdObj = new ModuleId(moduleId);
            var lessonIdObj = new LessonId(lessonId);
            var command = new GenerateLessonWithAiCommand(moduleIdObj, lessonIdObj);

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
