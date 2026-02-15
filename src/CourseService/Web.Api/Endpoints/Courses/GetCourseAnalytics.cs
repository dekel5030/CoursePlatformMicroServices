using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Features.Management.GetCourseAnalytics;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetCourseAnalytics : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("manage/courses/{id:Guid}/analytics", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCourseAnalyticsQuery(id);

            Result<CourseDetailedAnalyticsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithName("GetCourseAnalytics")
        .WithMetadata<CourseDetailedAnalyticsDto>(
            nameof(GetCourseAnalytics),
            tag: Tags.Courses,
            summary: "Gets detailed analytics for a course (instructor only).")
        .RequireAuthorization();
    }
}
