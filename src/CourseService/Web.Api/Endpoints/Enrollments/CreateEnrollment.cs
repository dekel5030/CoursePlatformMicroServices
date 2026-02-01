using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Commands.CreateEnrollment;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class CreateEnrollment : IEndpoint
{
    internal sealed record CreateEnrollmentRequest(
        Guid CourseId,
        Guid StudentId,
        DateTimeOffset? EnrolledAt,
        DateTimeOffset? ExpiresAt);

    internal sealed record CreateResponse(Guid EnrollmentId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("enrollments", async (
            CreateEnrollmentRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateEnrollmentCommand(
                new CourseId(request.CourseId),
                new UserId(request.StudentId),
                request.EnrolledAt,
                request.ExpiresAt);

            Result<CreateEnrollmentResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                response => Results.Created($"/enrollments/{response.EnrollmentId}", new CreateResponse(response.EnrollmentId)),
                CustomResults.Problem);
        })
        .WithMetadata<CreateResponse>(
            nameof(CreateEnrollment),
            tag: Tags.Enrollments,
            summary: "Creates a new enrollment (for development).",
            successStatusCode: StatusCodes.Status201Created);
    }
}
