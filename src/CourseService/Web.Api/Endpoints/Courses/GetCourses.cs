using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetCourses;
using Course.Api.Extensions;
using Course.Api.Infrastructure;
using Kernel.Messaging.Abstractions;

namespace Course.Api.Endpoints.Courses;

public class GetCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses", async (
            [AsParameters] PagedQueryDto pagedQuery,
            IQueryHandler<GetCoursesQuery, PagedResponseDto<CourseReadDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCoursesQuery(pagedQuery);
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
