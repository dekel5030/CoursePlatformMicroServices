using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCourseByIdQuery(id);

            Result<CourseDetailsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetCourseById")
        .WithTags(Tags.Courses);
    }
}