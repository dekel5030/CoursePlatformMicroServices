
using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetFeatured;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

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
