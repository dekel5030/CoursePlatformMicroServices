using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.CreateCourse;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

public class CreateCourse : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses", async (
            CreateCourseCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            Result<CreateCourseResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                response => Results.CreatedAtRoute(
                    nameof(GetCourseById),
                    new { id = response.CourseId.Value },
                    response
                ),
                CustomResults.Problem);
        })
        .WithMetadata<CreateCourseResponse>(
            nameof(CreateCourse),
            tag: Tags.Courses,
            summary: "Creates a new course.",
            successStatusCode: StatusCodes.Status201Created);
    }
}