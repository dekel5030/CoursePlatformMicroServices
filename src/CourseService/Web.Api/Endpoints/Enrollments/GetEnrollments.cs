using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Dtos;
using Courses.Application.Enrollments.Queries.GetEnrollments;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class GetEnrollments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("enrollments", async (
            IMediator mediator,
            CancellationToken cancellationToken,
            Guid? courseId = null,
            Guid? studentId = null,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetEnrollmentsQuery(courseId, studentId, pageNumber, pageSize);

            Result<EnrollmentCollectionDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<EnrollmentCollectionDto>(
            nameof(GetEnrollments),
            tag: Tags.Enrollments,
            summary: "Gets enrollments with optional filters by course and student.");
    }
}
