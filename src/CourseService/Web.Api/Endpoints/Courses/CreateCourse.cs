using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Courses.Commands.CreateCourse;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class CreateCourse : IEndpoint
{
    internal sealed record CreateCourseRequest(
        string? Title,
        string? Description);

    internal sealed record CreateResponse(Guid CourseId, string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses", async (
            CreateCourseRequest request,
            IMediator mediator,
            LinkProvider linkProvider,
            CancellationToken cancellationToken) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);

            var command = new CreateCourseCommand(title, description);

            Result<CreateCourseResponse> result = await mediator.Send(command, cancellationToken);

            return result.Match(
                course => Results.CreatedAtRoute(
                    nameof(GetCourseById),
                    new { id = course.Id.Value },
                    new CreateResponse(course.Id.Value, course.Title.Value)
                ),
                CustomResults.Problem);
        })
        .WithMetadata<CreateResponse>(
            nameof(CreateCourse),
            tag: Tags.Courses,
            summary: "Creates a new course.",
            successStatusCode: StatusCodes.Status201Created);
    }
}
