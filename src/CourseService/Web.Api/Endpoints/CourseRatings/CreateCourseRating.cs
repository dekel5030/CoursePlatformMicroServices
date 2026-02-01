using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.CreateCourseRating;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.CourseRatings;

internal sealed class CreateCourseRating : IEndpoint
{
    internal sealed record CreateCourseRatingRequest(int Score, string? Comment);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{courseId:Guid}/ratings", async (
            Guid courseId,
            CreateCourseRatingRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var courseIdObj = new CourseId(courseId);

            var command = new CreateCourseRatingCommand(
                courseIdObj,
                request.Score,
                request.Comment);

            Result<CreateCourseRatingResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                response => Results.CreatedAtRoute(
                    nameof(GetCourseRatings),
                    new { courseId = response.CourseId },
                    result.Value
                ),
                CustomResults.Problem);
        })
        .WithMetadata<CreateCourseRatingResponse>(
            nameof(CreateCourseRating),
            tag: Tags.CourseRatings,
            summary: "Creates a new rating for a course.",
            successStatusCode: StatusCodes.Status201Created);
    }
}
