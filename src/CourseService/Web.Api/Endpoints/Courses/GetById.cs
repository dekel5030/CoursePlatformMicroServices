using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetById;
using Course.Api.Endpoints;
using Course.Api.Extensions;
using Course.Api.Infrastructure;
using Domain.Courses.Primitives;
using Kernel;

namespace Course.Api.Endpoints.Courses;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/courses/{id:Guid}", async (
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