using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.DeleteLesson;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class DeleteLesson : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("modules/{moduleId:Guid}/lessons/{lessonId:Guid}", async (
            Guid moduleId,
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLessonCommand(new ModuleId(moduleId), new LessonId(lessonId));
            Result result = await mediator.Send(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(DeleteLesson),
            Tags.Lessons,
            "Deletes a specific lesson from a given course.",
            204);
    }
}
