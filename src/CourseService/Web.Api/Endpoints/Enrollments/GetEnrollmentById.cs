using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Dtos;
using Courses.Application.Enrollments.Queries.GetEnrollmentById;
using Courses.Domain.Enrollments.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class GetEnrollmentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("enrollments/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetEnrollmentByIdQuery(new EnrollmentId(id));

            Result<EnrollmentDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<EnrollmentDto>(
            nameof(GetEnrollmentById),
            tag: Tags.Enrollments,
            summary: "Gets an enrollment by ID.");
    }
}
