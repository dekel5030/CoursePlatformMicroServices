using Common.Auth;
using Common.Web.Errors;
using Common.Web.Extensions;
using CourseService.Dtos.Courses;
using CourseService.Extentions;
using CourseService.Services;
using Microsoft.AspNetCore.Mvc;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses");

        group.MapGet("/{courseId:int}", GetCourseById);
        group.MapGet("", SearchCourses);
        group.MapPost("", AddCourse).RequirePermission(PermissionType.CanCreateCourse);
        group.MapDelete("/{courseId:int}", DeleteCourse).RequirePermission(PermissionType.CanDeleteCourse);

        return app;
    }

    private static async Task<IResult> GetCourseById(int courseId, ICourseService courseService, ProblemDetailsFactory problemFactory)
    {
        var result = await courseService.GetCourseByIdAsync(courseId, true);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.ToProblemDetails(problemFactory));
    }

    private static async Task<IResult> SearchCourses([AsParameters] CourseSearchDto query, ICourseService courseService)
    {
        var result = await courseService.SearchCoursesAsync(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddCourse([FromBody] CourseCreateDto course, ICourseService courseService, ProblemDetailsFactory problemFactory)
    {
        var result = await courseService.AddCourseAsync(course);
        return result.IsSuccess
            ? Results.Created($"/api/courses/{result.Value!.Id}", result.Value)
            : Results.Problem(result.ToProblemDetails(problemFactory));
    }

    private static async Task<IResult> DeleteCourse(int courseId, ICourseService courseService, ProblemDetailsFactory problemFactory)
    {
        var result = await courseService.DeleteCourseAsync(courseId);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(result.ToProblemDetails(problemFactory));
    }
}
