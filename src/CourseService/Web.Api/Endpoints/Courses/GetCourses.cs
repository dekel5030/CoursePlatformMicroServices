using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetCourses;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Courses;

public class GetCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/courses", async (
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
