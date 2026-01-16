using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.GenerateThumbnailUploadUrl;
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
        app.MapPost("/courses/{id:guid}/image-upload-url", async (
            Guid id,
            GenerateUploadUrlRequest request,
            IMediator mediator) =>
        {
            var courseId = new CourseId(id);
            var command = new GenerateCourseImageUploadUrlCommand(courseId, request.FileName, request.ContentType);
            Result<GenerateUploadUrlResponse> result = await mediator.Send(command);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<GenerateUploadUrlResponse>(
            nameof(GenerateCourseImageUploadUrl), 
            Tags.Courses, 
            "Generate Course Image Upload URL");
    }
}
