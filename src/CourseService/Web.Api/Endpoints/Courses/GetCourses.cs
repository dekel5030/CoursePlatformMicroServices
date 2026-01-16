using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetCourses;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses", async (
            [AsParameters] PagedQueryDto pagedQuery,
            IMediator mediator,
            LinkProvider linksProvider,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCoursesQuery(pagedQuery);
            Result<CourseCollectionDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract(linksProvider, pagedQuery)),
                CustomResults.Problem);
        })
        .WithMetadata<PagedResponse<CourseSummaryResponse>>(
            nameof(GetCourses),
            tag: Tags.Courses,
            summary: "Gets a paginated list of courses.");
    }
}