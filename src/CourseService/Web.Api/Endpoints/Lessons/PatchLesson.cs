using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.PatchLesson;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

public class PatchLesson : IEndpoint
{
    public record PatchLessonRequest(
        string? Title,
        string? Description,
        string? Access,
        string? ThumbnailImageUrl,
        string? VideoUrl,
        TimeSpan? Duration);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("lessons/{id:guid}", async (
            Guid id,
            PatchLessonRequest request,
            IMediator mediator) =>
        {
            var command = new PatchLessonCommand(
                id,
                request.Title,
                request.Description,
                request.Access,
                request.ThumbnailImageUrl,
                request.VideoUrl,
                request.Duration);

            Result result = await mediator.Send(command);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithName(nameof(PatchLesson))
        .WithTags(Tags.Lessons)
        .WithSummary("Partially updates a lesson with only the fields provided.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
