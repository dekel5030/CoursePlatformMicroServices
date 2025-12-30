using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Courses.Commands.CreateCourse;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

public class CreateCourse : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses", async (
            CreateCourseCommand command,
            IMediator mediator) =>
        {
            Guid? instructorId = null;
            if (Guid.TryParse(dto.InstructorId, out var id))
            {
                instructorId = id;
            }

            var command = new CreateCourseCommand(
                dto.Title, 
                dto.Description, 
                instructorId, 
                dto.PriceAmount, 
                dto.PriceCurrency);

            var result = await mediator.Send(command);

            return result.Match(
                response => Results.CreatedAtRoute(
                "GetCourseById",
                new { id = response.CourseId },
                new { Id = response.CourseId }
            ),
            CustomResults.Problem);
        });
    }
}
