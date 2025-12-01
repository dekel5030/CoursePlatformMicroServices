
using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetFeatured;
using Course.Api.Endpoints;
using Course.Api.Extensions;
using Course.Api.Infrastructure;

namespace Course.Api.Endpoints.Courses;

public class GetFeatured : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/featured", async (
            IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetFeaturedQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
