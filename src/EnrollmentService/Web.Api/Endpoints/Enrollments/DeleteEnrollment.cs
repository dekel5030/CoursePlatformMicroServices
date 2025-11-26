using Application.Abstractions.Messaging;
using Application.Enrollments.Commands.DeleteEnrollment;
using Domain.Enrollments.Primitives;
using Enrollments.Api.Endpoints;
using Enrollments.Api.Extensions;
using Enrollments.Api.Infrastructure;

public class DeleteEnrollmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/enrollments/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteEnrollmentCommand> handler) =>
        {
            var enrollemntId = new EnrollmentId(id);
            var result = await handler.Handle(new DeleteEnrollmentCommand(enrollemntId));

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithName("DeleteEnrollment")
        .WithTags("Enrollments");
    }
}
