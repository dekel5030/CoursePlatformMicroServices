using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.PatchLesson;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class PatchLesson : IEndpoint
{
    internal sealed record PatchLessonRequest(
        string? Title,
        string? Description,
        LessonAccess? Access);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("modules/{moduleId:Guid}/lessons/{lessonId:Guid}", async (
            Guid moduleId,
            Guid lessonId,
            PatchLessonRequest request,
            IMediator mediator) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);

            var command = new PatchLessonCommand(
                new ModuleId(moduleId),
                new LessonId(lessonId),
                title,
                description,
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
