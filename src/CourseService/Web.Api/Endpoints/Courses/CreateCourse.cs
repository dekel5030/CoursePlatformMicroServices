
using Application.Abstractions.Messaging;
using Application.Courses.Commands.CreateCourse;
using Domain.Courses.Primitives;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Courses;

public class CreateCourse : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/courses", async (
            [AsParameters] CreateCourseDto dto,
            ICommandHandler<CreateCourseCommand, CourseId> handler) =>
        {
            var result = await handler.Handle(new CreateCourseCommand(dto));

            return result.Match(
                courseId => Results.CreatedAtRoute(
                "GetCourseById",
                new { id = courseId.Value },
                new { Id = courseId.Value }
            ),
            CustomResults.Problem);
        });
    }
}
