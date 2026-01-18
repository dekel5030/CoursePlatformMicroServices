
using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Endpoints.Courses;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GenerateLessonVideoUploadUrl : IEndpoint
{
    internal sealed record GenerateLessonVideoUploadUrlRequest(string FileName, string ContentType);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{courseidRaw:Guid}/lessons/{lessonIdRaw:Guid}/video-upload-url", async (
            Guid courseidRaw,
            Guid lessonIdRaw,
            GenerateLessonVideoUploadUrlRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            CourseId courseId = new(courseidRaw);
            LessonId lessonId = new(lessonIdRaw);
            var command = new GenerateLessonVideoUploadUrlCommand(
                courseId, lessonId, request.FileName, request.ContentType);

            Result<GenerateUploadUrlDto> result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<GenerateUploadUrlDto>(
            nameof(GenerateLessonVideoUploadUrl),
            Tags.Lessons,
            "Generate Lesson Video Upload URL");
    }
}
