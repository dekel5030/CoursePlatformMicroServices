using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Application.Courses.Queries.GetCourses;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

public class GetCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses", async (
            [AsParameters] PagedQueryDto pagedQuery,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCoursesQuery(pagedQuery);
            var result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract()),
                CustomResults.Problem);
        })
        .WithMetadata<PagedResponse<CourseSummaryResponse>>(
            nameof(GetCourses),
            tag: Tags.Courses,
            summary: "Gets a paginated list of courses.");
    }
}
