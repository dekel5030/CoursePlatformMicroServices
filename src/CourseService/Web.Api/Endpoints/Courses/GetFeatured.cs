using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetFeatured;
using Kernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Courses;

internal sealed class GetFeatured : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/courses/featured", async (
            IQueryHandler<GetFeaturedCoursesQuery, IEnumerable<CourseReadDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFeaturedCoursesQuery();

            Result<IEnumerable<CourseReadDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetFeaturedCourses")
        .WithTags(Tags.Courses);
    }
}
