using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetManagedCourses;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetManagedCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/me/courses/managed", async (
            IMediator mediator,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetManagedCoursesQuery(pageNumber, pageSize);

            Result<CourseCollectionDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithName("GetManagedCourses")
        .WithMetadata<CourseCollectionDto>(
            nameof(GetManagedCourses),
            tag: Tags.Courses,
            summary: "Gets the list of courses managed by the current user (instructor).")
        .RequireAuthorization();
    }
}
