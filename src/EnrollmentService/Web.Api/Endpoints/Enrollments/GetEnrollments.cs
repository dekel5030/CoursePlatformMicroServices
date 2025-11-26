using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Application.Enrollments.Queries.GetEnrollments;
using Enrollments.Api.Endpoints;
using Enrollments.Api.Extensions;
using Enrollments.Api.Infrastructure;

public class GetEnrollmentsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/enrollments", async (
            [AsParameters] GetEnrollmentsQueryParams queryParams,
            IQueryHandler<GetEnrollmentsQuery, PagedResponse<EnrollmentReadDto>> handler) =>
        {
            var query = new GetEnrollmentsQuery(queryParams);

            var result = await handler.Handle(query);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetEnrollments")
        .WithTags("Enrollments");
    }
}
