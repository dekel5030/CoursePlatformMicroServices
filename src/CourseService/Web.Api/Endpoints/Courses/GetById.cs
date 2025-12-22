using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{id:Guid}", async (
            Guid id,
            IQueryHandler<GetCourseByIdQuery, CourseReadDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCourseByIdQuery(new CourseId(id));

            Result<CourseReadDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetCourseById")
        .WithTags(Tags.Courses);
    }
}