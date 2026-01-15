using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Lessons;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Lessons.Commands.CreateLesson;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class CreateLesson : IEndpoint
{
    internal sealed record CreateLessonRequest(string? Title, string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{courseid:Guid}/lessons", async (
            Guid courseid,
            CreateLessonRequest request,
            IMediator mediator,
            LinkProvider linkProvider) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);
            var courseIdObj = new CourseId(courseid);

            var command = new CreateLessonCommand(
                courseIdObj,
                title,
                description);

            Result<LessonDetailsDto> result = await mediator.Send(command);

            return result.Match(
                lessonDto => Results.CreatedAtRoute(
                    nameof(GetLessonById),
                    new { courseId = courseid, lessonId = lessonDto.LessonId.Value },
                    lessonDto.ToApiContract(courseIdObj, linkProvider)
                ),
                CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsResponse>(
            nameof(CreateLesson), 
            Tags.Lessons, 
            "Create a lesson", 
            StatusCodes.Status201Created);
    }
}
