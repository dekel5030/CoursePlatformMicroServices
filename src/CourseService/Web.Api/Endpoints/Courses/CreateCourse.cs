using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.CreateCourse;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

public class CreateCourse : IEndpoint
{
    public record CreateCourseRequest(
        string? Title,
        string? Description,
        Guid? InstructorId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses", async (
            CreateCourseRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);

            var command = new CreateCourseCommand(title, description, request.InstructorId);

            Result<CreateCourseResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                response => Results.CreatedAtRoute(
                    nameof(GetCourseById),
                    new { id = response.CourseId.Value },
                    response.ToApiContract()
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