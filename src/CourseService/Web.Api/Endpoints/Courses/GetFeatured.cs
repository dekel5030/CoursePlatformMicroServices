
using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetFeatured;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Courses;

public class GetFeatured : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/courses/featured", async (
            IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetFeaturedQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
