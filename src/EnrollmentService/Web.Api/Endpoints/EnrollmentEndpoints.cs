using Application.Abstractions.Messaging;
using Application.Enrollments.Commands.CreateEnrollment;
using Application.Enrollments.Commands.DeleteEnrollment;
using Application.Enrollments.Queries.Dtos;
using Application.Enrollments.Queries.GetEnrollmentById;
using Application.Enrollments.Queries.GetEnrollments;
using Domain.Enrollments.Primitives;
using Kernel;

namespace Web.Api.Endpoints;

public static class EnrollmentEndpoints
{
    public static IEndpointRouteBuilder MapEnrollmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/enrollments")
            .WithTags("Enrollments")
            .WithOpenApi();

        group.MapGet("/{id:int}", GetEnrollmentById)
            .WithName("GetEnrollmentById")
            .Produces<EnrollmentReadDto>()
            .Produces(404);

        group.MapGet("/", GetEnrollments)
            .Produces<PagedResponse<EnrollmentReadDto>>();

        group.MapPost("/", CreateEnrollment)
            .Produces<EnrollmentReadDto>(201)
            .Produces(400)
            .Produces(409);

        group.MapDelete("/{id:int}", DeleteEnrollment)
            .Produces(204)
            .Produces(404);

        return app;
    }

    private static async Task<IResult> GetEnrollmentById(
        int id,
        IQueryHandler<GetEnrollmentByIdQuery, EnrollmentReadDto> handler)
    {
        var query = new GetEnrollmentByIdQuery(id);
        var result = await handler.Handle(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                statusCode: result.Error!.Type == ErrorType.NotFound ? 404 : 400,
                title: result.Error.Code,
                detail: result.Error.Description);
    }

    private static async Task<IResult> GetEnrollments(
        [AsParameters] GetEnrollmentsRequest request,
        IQueryHandler<GetEnrollmentsQuery, PagedResponse<EnrollmentReadDto>> handler)
    {
        var query = new GetEnrollmentsQuery(
            request.UserId,
            request.CourseId,
            request.Status,
            request.PageNumber,
            request.PageSize);

        var result = await handler.Handle(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                statusCode: 400,
                title: result.Error!.Code,
                detail: result.Error.Description);
    }

    private static async Task<IResult> CreateEnrollment(
        CreateEnrollmentRequest request,
        ICommandHandler<CreateEnrollmentCommand, EnrollmentId> handler)
    {
        var command = new CreateEnrollmentCommand(
            request.UserId,
            request.CourseId,
            request.ExpiresAt);

        var result = await handler.Handle(command);

        return result.IsSuccess
            ? Results.CreatedAtRoute(
                "GetEnrollmentById",
                new { id = result.Value!.Value },
                new EnrollmentReadDto
                {
                    Id = result.Value.Value,
                    UserId = command.UserId,
                    CourseId = command.CourseId,
                    Status = EnrollmentStatus.Pending,
                    EnrolledAt = DateTime.UtcNow,
                    ExpiresAt = command.ExpiresAt
                })
            : Results.Problem(
                statusCode: result.Error!.Type == ErrorType.Conflict ? 409 : 400,
                title: result.Error.Code,
                detail: result.Error.Description);
    }

    private static async Task<IResult> DeleteEnrollment(
        int id,
        ICommandHandler<DeleteEnrollmentCommand> handler)
    {
        var command = new DeleteEnrollmentCommand(id);
        var result = await handler.Handle(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(
                statusCode: result.Error!.Type == ErrorType.NotFound ? 404 : 400,
                title: result.Error.Code,
                detail: result.Error.Description);
    }
}

public record GetEnrollmentsRequest(
    int? UserId = null,
    int? CourseId = null,
    EnrollmentStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10);

public record CreateEnrollmentRequest(
    int UserId,
    int CourseId,
    DateTime? ExpiresAt = null);
