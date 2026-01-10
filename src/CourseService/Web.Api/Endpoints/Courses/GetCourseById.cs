using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.Extensions;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetCourseById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCourseByIdQuery(id.MapValueObject<CourseId>());

            Result<CourseDetailsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<CourseDetailsDto>(
            nameof(GetCourseById),
            tag: Tags.Courses,
            summary: "Gets a course by its ID.");
    }
}