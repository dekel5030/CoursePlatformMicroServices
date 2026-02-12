using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.CourseRatings.Commands.UpdateCourseRating;
using Courses.Domain.Ratings.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.CourseRatings;

internal sealed class UpdateCourseRating : IEndpoint
{
    internal sealed record UpdateCourseRatingRequest(int? Score, string? Comment);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("course-ratings/{ratingId:Guid}", async (
            Guid ratingId,
            UpdateCourseRatingRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCourseRatingCommand(
                new RatingId(ratingId),
                request.Score,
                request.Comment);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(UpdateCourseRating),
            Tags.CourseRatings,
            "Partially updates a course rating with only the fields provided.");
    }
}
