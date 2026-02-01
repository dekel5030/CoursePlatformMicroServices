using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Dtos;
using Courses.Application.Enrollments.Queries.GetEnrolledCourses;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class GetEnrolledCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/me/enrollments", async (
            IMediator mediator,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetEnrolledCoursesQuery(pageNumber, pageSize);

            Result<EnrolledCourseCollectionDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithName("GetEnrolledCourses")
        .WithMetadata<EnrolledCourseCollectionDto>(
            nameof(GetEnrolledCourses),
            tag: Tags.Enrollments,
            summary: "Gets the list of courses the current user is enrolled in, with progress and last-accessed metadata.")
        .RequireAuthorization();
    }
}
