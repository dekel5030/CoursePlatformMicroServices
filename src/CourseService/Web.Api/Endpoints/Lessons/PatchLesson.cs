using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.PatchLesson;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

public class PatchLesson : IEndpoint
{
    public record PatchLessonRequest(
        string? Title,
        string? Description,
        LessonAccess? Access);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("courses/{courseId:Guid}/lessons/{lessonId:Guid}", async (
            Guid courseId,
            Guid lessonId,
            PatchLessonRequest request,
            IMediator mediator) =>
        {
            var command = new PatchLessonCommand(
                new CourseId(courseId),
                new LessonId(lessonId),
                request.Title,
                request.Description,
                request.Access);

            Result result = await mediator.Send(command);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(PatchLesson), 
            Tags.Lessons, 
            "Partially updates a lesson with only the fields provided.");
    }
}
