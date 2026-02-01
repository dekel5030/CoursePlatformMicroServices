using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Enrollments.Commands.DeleteEnrollment;
using Courses.Domain.Enrollments.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Enrollments;

internal sealed class DeleteEnrollment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("enrollments/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteEnrollmentCommand(new EnrollmentId(id));
            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(nameof(DeleteEnrollment), Tags.Enrollments, "Deletes an enrollment.", 204);
    }
}
