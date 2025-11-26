using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Application.Enrollments.Queries.GetEnrollmentById;
using Domain.Enrollments.Primitives;
using Enrollments.Api.Extensions;
using Enrollments.Api.Infrastructure;

namespace Enrollments.Api.Endpoints.Enrollments;

public class GetEnrollmentByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/enrollments/{id:int}", async (
            Guid id,
            IQueryHandler<GetEnrollmentByIdQuery, EnrollmentReadDto> handler) =>
        {
            var enrollmentId = new EnrollmentId(id);
            var result = await handler.Handle(new GetEnrollmentByIdQuery(enrollmentId));

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetEnrollmentById")
        .WithTags("Enrollments");
    }
}