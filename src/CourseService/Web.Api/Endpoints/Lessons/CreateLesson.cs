using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.Extensions;
using Courses.Application.Lessons.Commands.CreateLesson;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

public class CreateLesson : IEndpoint
{
    public record CreateLessonRequest(string? Title, string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("courses/{courseid:Guid}/lessons", async (
            Guid courseid,
            CreateLessonRequest request,
            IMediator mediator) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);

            var command = new CreateLessonCommand(
                courseid.MapValueObject<CourseId>(),
                title,
                description);

            Result<LessonDetailsDto> result = await mediator.Send(command);

            return result.Match(
                lessonDto => Results.CreatedAtRoute(
                    nameof(GetLessonById),
                    new { courseId = courseid, lessonId = lessonDto.LessonId.Value },
                    lessonDto
                ),
                CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsDto>(
            nameof(CreateLesson), 
            Tags.Lessons, 
            "Create a lesson", 
            StatusCodes.Status201Created);
    }
}
