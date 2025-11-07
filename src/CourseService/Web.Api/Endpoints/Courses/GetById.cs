using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetById;
using Domain.Courses.Primitives;
using Kernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Orders;

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