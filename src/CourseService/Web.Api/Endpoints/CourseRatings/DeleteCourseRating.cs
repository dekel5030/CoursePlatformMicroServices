using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.DeleteCourseRating;
using Courses.Domain.Ratings.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.CourseRatings;

internal sealed class DeleteCourseRating : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("course-ratings/{ratingId:Guid}", async (
            Guid ratingId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteCourseRatingCommand(new RatingId(ratingId));

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(DeleteCourseRating),
            Tags.CourseRatings,
            "Deletes a course rating. Only the rating owner can delete.");
    }
}
