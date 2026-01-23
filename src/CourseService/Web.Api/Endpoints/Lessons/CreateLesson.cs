using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Endpoints.Modules;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Lessons.Commands.CreateLesson;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class CreateLesson : IEndpoint
{
    internal sealed record CreateLessonRequest(string? Title, string? Description);

    private sealed record CreateResponse(Guid ModuleId, Guid CourseId, string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("modules/{moduleId:Guid}/lessons", async (
            Guid moduleId,
            CreateLessonRequest request,
            IMediator mediator) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);
            var moduleIdObj = new ModuleId(moduleId);

            var command = new CreateLessonCommand(
                moduleIdObj,
                title,
                description);

            Result<CreateLessonResponse> result = await mediator.Send(command);

            return result.Match(
                lessonDto => Results.CreatedAtRoute(
                    nameof(GetModulesByCourseId),
                    new { courseId = lessonDto.CourseId.Value },
                    result.Value
                ),
                CustomResults.Problem);
        })
        .WithMetadata<CreateLessonResponse>(
            nameof(CreateLesson),
            Tags.Lessons,
            "Create a lesson",
            StatusCodes.Status201Created);
    }
}
