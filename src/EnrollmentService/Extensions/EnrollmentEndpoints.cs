using Common.Auth;
using Common.Auth.Extentions;
using Common.Messaging.EventEnvelope;
using Common.Web.Errors;
using EnrollmentService.Dtos;
using EnrollmentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Extensions;

public static class EnrollmentEndpoints
{
    public static IEndpointRouteBuilder MapEnrollmentEndpoints(this IEndpointRouteBuilder routes)
    {
        routes = routes.MapAdminEndpoints();

        return routes;
    }

    private static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/admin/enrollments");//.RequirePermission(PermissionType.CanModifyEnrollment);

        group.MapGet("/{id:int}", GetEnrollmentById).WithName("GetEnrollmentById");
        group.MapGet("/", SearchEnrollments);
        group.MapPost("/", CreateEnrollment);
        group.MapDelete("/{id:int}", DeleteEnrollment);

        return routes;
    }

    private static async Task<IResult> GetEnrollmentById(
        int id,
        IEnrollmentService enrollmentService,
        [FromServices] ProblemDetailsFactory problemFactory,
        CancellationToken ct = default)
    {
        var result = await enrollmentService.GetEnrollmentByIdAsync(id, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(problemFactory.Create(result.Error!));
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> SearchEnrollments(
        [AsParameters] EnrollmentSearchDto searchDto,
        IEnrollmentService enrollmentService,
        CancellationToken ct = default)
    {
        var response = await enrollmentService.SearchEnrollmentsAsync(searchDto, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateEnrollment(
        EnrollmentCreateDto enrollmentCreateDto,
        IEnrollmentService enrollmentService,
        [FromServices] ProblemDetailsFactory problemFactory,
        CancellationToken ct = default)
    {
        var result = await enrollmentService.CreateEnrollmentAsync(enrollmentCreateDto, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(problemFactory.Create(result.Error!));
        }

        return Results.CreatedAtRoute(
            "GetEnrollmentById",
            new { id = result.Value!.Id },
            result.Value);
    }

    private static async Task<IResult> DeleteEnrollment(
        int id,
        IEnrollmentService enrollmentService,
        [FromServices] ProblemDetailsFactory problemFactory,
        CancellationToken ct = default)
    {
        var result = await enrollmentService.DeleteEnrollmentAsync(id, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(problemFactory.Create(result.Error!));
        }

        return Results.NoContent();
    }
}