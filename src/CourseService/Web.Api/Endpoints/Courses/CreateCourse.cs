using Application.Courses.Commands.CreateCourse;
using Course.Api.Extensions;
using Course.Api.Infrastructure;
using Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Course.Api.Endpoints.Courses;

public class CreateCourse : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses", async (
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
