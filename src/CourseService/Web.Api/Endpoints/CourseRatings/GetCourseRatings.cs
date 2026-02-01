using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCourseRatings;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.CourseRatings;

internal sealed class GetCourseRatings : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{courseId:Guid}/ratings", async (
            Guid courseId,
            IMediator mediator,
            IUserContext userContext,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetCourseRatingsQuery(courseId, pageNumber, pageSize);

            Result<CourseRatingCollection> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<CourseRatingCollection>(
            nameof(GetCourseRatings),
            tag: Tags.CourseRatings,
            summary: "Gets all ratings for a course by course ID.");
    }
}
