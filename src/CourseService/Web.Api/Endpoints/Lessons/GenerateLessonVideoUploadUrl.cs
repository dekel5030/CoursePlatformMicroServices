using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GenerateLessonVideoUploadUrl : IEndpoint
{
    internal sealed record GenerateLessonVideoUploadUrlRequest(string FileName, string ContentType);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("modules/{moduleId:Guid}/lessons/{lessonId:Guid}/video-upload-url", async (
            Guid moduleId,
            Guid lessonId,
            GenerateLessonVideoUploadUrlRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            ModuleId moduleIdObj = new(moduleId);
            LessonId lessonIdObj = new(lessonId);
            var command = new GenerateLessonVideoUploadUrlCommand(
                moduleIdObj, lessonIdObj, request.FileName, request.ContentType);

            Result<GenerateUploadUrlDto> result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<GenerateUploadUrlDto>(
            nameof(GenerateLessonVideoUploadUrl),
            Tags.Lessons,
            "Generate Lesson Video Upload URL");
    }
}
