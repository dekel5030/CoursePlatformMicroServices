using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.MoveLesson;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class MoveLesson : IEndpoint
{
    internal sealed record MoveLessonRequest(Guid TargetModuleId, int TargetIndex);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("lessons/{lessonId:Guid}/move", async (
            Guid lessonId,
            MoveLessonRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            MoveLessonCommand command = new(
                new LessonId(lessonId),
                new ModuleId(request.TargetModuleId),
                request.TargetIndex);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(MoveLesson),
            Tags.Lessons,
            "Moves a lesson to a different module.");
    }
}
