using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Courses;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
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
            LinkProvider linkProvider,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCourseByIdQuery(new CourseId(id));

            Result<CoursePageDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),//.ToApiContract(linkProvider)),
                CustomResults.Problem);
        })
        .WithMetadata<CourseDetailsResponse>(
            nameof(GetCourseById),
            tag: Tags.Courses,
            summary: "Gets a course by its ID.");
    }
}
