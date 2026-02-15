using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Features.Management;
using Courses.Application.Features.Management.ManagedCourses;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Manage;

internal sealed class GetManagedCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("manage/courses", async (
            IMediator mediator,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetManagedCoursesQuery(pageNumber, pageSize);

            Result<PaginatedCollectionDto<ManagedCourseSummaryDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithName("GetManagedCourses")
        .WithMetadata<PaginatedCollectionDto<ManagedCourseSummaryDto>>(
            nameof(GetManagedCourses),
            tag: Tags.Courses,
            summary: "Gets the list of courses managed by the current user (instructor).")
        .RequireAuthorization();
    }
}
