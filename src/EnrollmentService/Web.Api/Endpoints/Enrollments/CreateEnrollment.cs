using Application.Abstractions.Messaging;
using Application.Enrollments.Commands.CreateEnrollment;
using Domain.Enrollments.Primitives;
using Enrollments.Api.Endpoints;
using Enrollments.Api.Extensions;
using Enrollments.Api.Infrastructure;

public class CreateEnrollmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/enrollments", async (
            CreateEnrollmentDto request,
            ICommandHandler<CreateEnrollmentCommand, EnrollmentId> handler) =>
        {
            var command = new CreateEnrollmentCommand(request);

            var result = await handler.Handle(command);

            return result.Match(
                id => Results.CreatedAtRoute(
                    "GetEnrollmentById",
                    new { id = id.Value },
                    new { id = id.Value }
                ),
                CustomResults.Problem
            );
        })
        .WithName("CreateEnrollment")
        .WithTags("Enrollments");
    }
}
