using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.GenerateCourseImageUploadUrl;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GenerateCourseImageUploadUrl : IEndpoint
{
    internal sealed record GenerateUploadUrlRequest(
        string FileName,
        string ContentType);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{id:Guid}/image-upload-url", async (
            Guid id,
            GenerateUploadUrlRequest request,
            IMediator mediator) =>
        {
            var courseId = new CourseId(id);
            var command = new GenerateCourseImageUploadUrlCommand(courseId, request.FileName, request.ContentType);
            Result<UploadUrlDto> result = await mediator.Send(command);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<UploadUrlDto>(
            nameof(GenerateCourseImageUploadUrl),
            Tags.Courses,
            "Generate Course Image Upload URL");
    }
}
