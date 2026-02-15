using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Features.Management.ManagedCoursePage;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Manage;

internal sealed class GetManagedCourseById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("manage/courses/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new ManagedCoursePageQuery(id);

            Result<ManagedCoursePageDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithName("GetManagedCourseById")
        .WithMetadata<ManagedCoursePageDto>(
            nameof(GetManagedCourseById),
            tag: Tags.Courses,
            summary: "Gets a course for management by the instructor.")
        .RequireAuthorization();
    }
}
